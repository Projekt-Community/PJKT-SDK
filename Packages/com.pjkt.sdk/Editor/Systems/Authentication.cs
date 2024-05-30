using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

namespace PJKT.SDK2.NET
{
    public static class Authentication
    {
        internal static PjktActiveUser ActiveUser = null;

        private static bool _isLoggedIn = false;
        internal static bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            private set
            {
                _isLoggedIn = value;
                //Debug.Log("login status changed");

                OnLoginStatusChanged(null, EventArgs.Empty);
            }
        }
        
        //handles notifying of login status
        public static event EventHandler OnLoginStatusChanged = delegate {  };
        
        //Firebase variables
        private const string AppOptionsPath = "Packages/com.pjkt.sdk/Editor/Firebase/google-services.json";
        //private const string devAppOptionsPath = "Packages/com.pjkt.sdk/Editor/Firebase/google-services_dev.json";
        private static readonly string accountDataPath = Application.persistentDataPath + "/Projekt Community/UnityEditor/";
        private readonly static FirebaseApp app; //Readonly
        private readonly static FirebaseAuth auth; //Readonly
        private static FirebaseUser previousUser;
        
        internal static string PjktCookie = "";
        //internal static string communityName = "";
        internal static bool isAuthorized { get { return auth != null && auth.CurrentUser != null; } }
        internal static string token { get; private set; }
        
        static Authentication()
        {
            //Firebase initialization
            string AppOptionsJsonString = File.ReadAllText(AppOptionsPath);
            AppOptions appOptions = AppOptions.LoadFromJsonConfig(AppOptionsJsonString);
            app = FirebaseApp.Create(appOptions, "pjkt-sdk");
            auth = FirebaseAuth.GetAuth(app);
        }

        internal static async Task TryResumeSession()
        {
            if (File.Exists(accountDataPath + "SessionData.pjkt"))
            {
                string sessionJson = File.ReadAllText(accountDataPath + "SessionData.pjkt");
                try
                {
                    PjktSessionInfo resumeSession = JsonUtility.FromJson<PjktSessionInfo>(sessionJson);
                    PjktCookie = resumeSession.SessionToken;
                }
                catch
                {
                    PjktSdkWindow.Notify("Error resuming session. Try logging back in.", BoothErrorType.Error);
                    Logout();
                    return;
                }
            }
            else return;
            
            //check if the session is still valid
            bool sessionValid = await GetActiveUserInfo();
            
            if (!sessionValid)
            {
                PjktSdkWindow.Notify("Session is invalid, please log in again.", BoothErrorType.Error);
                Logout();
                return;
            }
                
            IsLoggedIn = true;
        }

        internal static async Task<bool> GetActiveUserInfo()
        {
            //Debug.Log($"Getting user info. PJKT Cookie: {PjktCookie}");
            
            //check pjkt
            if (PjktCookie == "") return false;
            string response = await PJKTNet.RequestMessage("/me");
            
            //Debug.Log($"get user info response: {response}");
            if (string.IsNullOrEmpty(response)) return false;
            
            if (response.Contains("Unauthorized! Verification Failed")) return false;
            
            //deserilize the json
            try { ActiveUser = JsonUtility.FromJson<PjktActiveUser>(response); }
            catch {return false;}

            //Debug.Log("Found user info:" + ActiveUser.user.username);

            return true;
        }
        
        

        internal static async Task Register(string userName, string email, string password, string inviteCode)
        {
            AuthResult result;
            try {result = await auth.CreateUserWithEmailAndPasswordAsync(email, password); }
            catch (Exception e)
            {
                PjktSdkWindow.Notify($"Register failed, please try again \n {e}", BoothErrorType.Error);
                return;
            }
                
            //get the token
            token = await auth.CurrentUser.TokenAsync(false);

            //create pjkt account
            PJKTAccountCreationMessage accountCreationMessage = new PJKTAccountCreationMessage(userName, inviteCode, token);
            HttpResponseMessage accountCreationResponse = await PJKTNet.SendMessage(accountCreationMessage);
                
            IEnumerable<string> cookieHeaders;
            if (accountCreationResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //get the session token
                if (accountCreationResponse.Headers.TryGetValues("set-cookie", out cookieHeaders))
                {
                    string[] headerdata = cookieHeaders.FirstOrDefault().Split(';');
                    string cookie = headerdata[0];

                    PjktCookie = cookie.Substring(8);
                }
                else
                {
                    PjktSdkWindow.Notify("Unable to create account.\n" + accountCreationResponse.StatusCode, BoothErrorType.Error);
                    auth.SignOut();
                    return;
                }

                //save session information
                PjktSessionInfo session = new PjktSessionInfo(PjktCookie);
                string savedSEssionData = JsonUtility.ToJson(session);
                if (!Directory.Exists(accountDataPath)) Directory.CreateDirectory(accountDataPath);
                File.WriteAllText(accountDataPath + "SessionData.pjkt", savedSEssionData);

                bool foundProfile = await GetActiveUserInfo();
                
                if (!foundProfile)
                {
                    PjktSdkWindow.Notify("Account created successfully, but error getting profile info. Try logging in", BoothErrorType.Error);
                    Logout();
                    return;
                }
                
                PjktSdkWindow.Notify("Account created successfully!", BoothErrorType.Info);
                IsLoggedIn = true;
            }
        }
        
        internal static async Task Login(string email, string password)
        {
            AuthResult result;
            try {result = await auth.SignInWithEmailAndPasswordAsync(email, password); }
            catch (Exception e)
            {
                PjktSdkWindow.Notify($"Login failed, please try again \n {e}", BoothErrorType.Error);
                return;
            }
                
            if (!isAuthorized)
            {
                PjktSdkWindow.Notify("Login failed, please try again", BoothErrorType.Error);
                return;
            }
                
            //get the token
            token = await auth.CurrentUser.TokenAsync(false);
            
            PJKTLoginMessage message = new PJKTLoginMessage();
            HttpResponseMessage pjktNetResponse = await PJKTNet.SendMessage(message);

            IEnumerable<string> cookieHeaders;
                
            //get the session token
            if (pjktNetResponse.Headers.TryGetValues("set-cookie", out  cookieHeaders))
            {
                string[] headerdata = cookieHeaders.FirstOrDefault().Split(';');
                string cookie = headerdata[0];
                
                PjktCookie = cookie.Substring(8);
            }
            else
            {
                PjktSdkWindow.Notify("Unable to login to PJKT Services, try restarting unity?", BoothErrorType.Error);
                auth.SignOut();
                return;
            }
            
            
            //save session information
            PjktSessionInfo session = new PjktSessionInfo(PjktCookie);
            string savedSEssionData = JsonUtility.ToJson(session);
            if (!Directory.Exists(accountDataPath)) Directory.CreateDirectory(accountDataPath);
            File.WriteAllText(accountDataPath + "SessionData.pjkt", savedSEssionData);
            
            bool foundProfile = await GetActiveUserInfo();  Debug.Log(foundProfile);
            
            if (!foundProfile)
            {
                PjktSdkWindow.Notify("Error getting profile info. Try logging in again", BoothErrorType.Error);
                Logout();
                return;
            }
            
            IsLoggedIn = true;
        }

        public static async void Logout()
        {
            auth.SignOut();
            
            PJKTLogoutMessage message = new PJKTLogoutMessage();
            await PJKTNet.SendMessage(message);

            //delete the session data
            if (File.Exists(accountDataPath + "SessionData.pjkt"))
            {
                File.Delete(accountDataPath + "SessionData.pjkt");
            }
            PjktCookie = "";
            IsLoggedIn = false;
        }
        
        public static async void ResetPassword(string email)
        {
            await auth.SendPasswordResetEmailAsync(email);
            
            PjktSdkWindow.Notify("Password reset email sent", BoothErrorType.Info);
        }
        
        public static async void JoinCommunity(string inviteCode)
        {
            PJKTJoinCommunityMessage joinCommunityMessage = new PJKTJoinCommunityMessage(inviteCode);
            HttpResponseMessage resposnse = await PJKTNet.SendMessage(joinCommunityMessage);
            
            if (resposnse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                PjktSdkWindow.Notify("Unable to join community", BoothErrorType.Error);
                return;
            }

            bool refreshUser = await GetActiveUserInfo();
            if (!refreshUser)
            {
                PjktSdkWindow.Notify("Unable to Get user profile but community may still have been added. Try logging out and back in", BoothErrorType.Error);
                return;
            }
            
            PjktSdkWindow.Notify("Community joined successfully!", BoothErrorType.Info);
        }
    }
    [Serializable]
    internal class PjktSessionInfo
    {
        public string SessionToken;

        public PjktSessionInfo(string sessionToken)
        {
            SessionToken = sessionToken;
        }
    }
}