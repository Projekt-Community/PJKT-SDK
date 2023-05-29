using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using UnityEngine;
using UnityEditor;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using PJKT.SDK.NET;
using PJKT.SDK.NET.Messages;
using System.IO;

namespace PJKT.SDK.Window
{
    /* This static class handles user authentication.
     * It is always running and cannot be instantiated or destroyed.
     * As a side effect of its static-y nature, it can always be referenced from anywhere.
     */
    internal static class AuthData
    {
        //Firebase variables
        private const string AppOptionsPath = "Packages/com.pjkt.sdk/Editor/Firebase/google-services.json";
        private static readonly string cookiePath = Application.persistentDataPath + "/Projekt Community/UnityEditor/";
        private readonly static FirebaseApp app; //Readonly
        private readonly static FirebaseAuth auth; //Readonly
        private static FirebaseUser previousUser;

        internal static bool isAuthorized { get { return auth != null && auth.CurrentUser != null; } }
        internal static string displayName { //Display name with a fallback to email or UID
            get {
                if (!isAuthorized) return "Unknown";
                else if (auth.CurrentUser.DisplayName != "") return auth.CurrentUser.DisplayName;
                else if (auth.CurrentUser.Email != "") return auth.CurrentUser.Email;
                else return auth.CurrentUser.UserId;
            }
            set {
                if (isAuthorized) {
                    auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile { DisplayName = value });
                }
            }
        }

        //----------Networked variables----------
        //Any changes to the below variables will be sent to the server instantly
        internal static string emailAddress { //Email address with a fallback to UID
            get {
                if (!isAuthorized) return "Unknown";
                else return auth.CurrentUser.Email;
            }
            set {
                if (isAuthorized) auth.CurrentUser.UpdateEmailAsync(value);
            }
        }
        internal static string password { //Password with a fallback to UID
            set {
                if (isAuthorized) auth.CurrentUser.UpdatePasswordAsync(value);
            }
        }

        internal static PJKTProfile _pjktProfile = new PJKTProfile();
        internal static PJKTProfile pjktProfile
        {
            get { return _pjktProfile; }
            set
            {
                _pjktProfile = value;
                Task sendProfileUpdate = PJKTNet.SendMessage(new PJKTProfile(value.CommunityName, value.DiscordURL, value.ProfilePhotoBytes));
            }
        }
        //----------End networked variables----------
        internal static string communityName {
            get{ return pjktProfile.CommunityName; }
        }
        internal static Texture2D loadProfilePhoto {
            get{
                Texture2D profilePhoto = new Texture2D(1, 1);
                profilePhoto.LoadImage(pjktProfile.ProfilePhotoBytes);
                return profilePhoto;
            }
        }
        internal static string discordURL {
            get{ return pjktProfile.DiscordURL; }
        }
        internal static string token {
            get
            {
                if (isAuthorized)
                {
                    Task<string> tokenTask = auth.CurrentUser.TokenAsync(false);
                    return tokenTask.Result;
                }
                else
                {
                    return "";
                }
            }
        }

        internal static string PjktCookie = "";
        
        internal static string loginMessage { get; private set; } = ""; //Contains the response from the server

        //Static constructor, called once on Unity start or recompile
        static AuthData()
        {
            //Firebase initialization
            string AppOptionsJsonString = System.IO.File.ReadAllText(AppOptionsPath);
            AppOptions appOptions = AppOptions.LoadFromJsonConfig(AppOptionsJsonString);
            app = FirebaseApp.Create(appOptions, "pjkt-sdk");
            auth = FirebaseAuth.GetAuth(app);
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(null, null);

            if (File.Exists(cookiePath + "cookies.pjkt")) PjktCookie = File.ReadAllText(cookiePath + "cookies.pjkt");
        }

        internal static void AuthStateChanged(object sender, System.EventArgs eventArgs) {
            FirebaseUser prevUser = previousUser;
            FirebaseUser currentUser = auth.CurrentUser;

            //Only do stuff if something actually changed
            if (prevUser != currentUser) {

                //User signed in
                if (currentUser != null && prevUser == null) {
                    //Debug.Log("Signed in as " + currentUser.UserId);
                }

                //User signed out
                if (currentUser == null && prevUser != null) {
                    //Debug.Log("Signed out " + prevUser.UserId);
                }

                //User switched accounts
                if (currentUser != null && prevUser != null) {
                    //Debug.Log("Switched accounts from " + prevUser.UserId + " to " + currentUser.UserId);
                }

                previousUser = auth.CurrentUser;

            }
        }

        static void OnDestroy() {
            auth.StateChanged -= AuthStateChanged;
        }

        //Login with email and password
        internal static Task Login(string email, string password, EditorWindow caller = null)
        {
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Logging in");
            return auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(async task => {
                if (task.IsCanceled) {
                    ErrorResponseHandler("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    ErrorResponseHandler("SignInWithEmailAndPasswordAsync encountered an error: \n" + task.Exception.Flatten().Message + " \nDid you enter the correct email and password?");
                    return;
                }

                await PJKTLogin();
                if (caller != null) caller.Repaint();
                //else Debug.Log("Did not repaint");
            });
        }

        internal static async Task PJKTLogin()
        {
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Contacting PJKT services...");
            
            PJKTLoginMessage loginMessage = new PJKTLoginMessage();
            HttpResponseMessage pjktNetResponse = await PJKTNet.SendMessage(loginMessage);

            IEnumerable<string> cookieHeaders;
                
            if (pjktNetResponse.Headers.TryGetValues("set-cookie", out  cookieHeaders))
            {
                string[] headerdata = cookieHeaders.FirstOrDefault().Split(';');
                string cookie = headerdata[0];
                
                PjktCookie = cookie.Substring(8);
                if (!Directory.Exists(cookiePath)) Directory.CreateDirectory(cookiePath);
                File.WriteAllText(cookiePath + "cookies.pjkt", PjktCookie);
               
                //logout when unity closes
                EditorApplication.quitting += Logout;
                
                Debug.Log("<color=#4557f7>PJKT SDK</color>: Logged in");
            }
            else
            {
                Debug.LogError("<color=#4557f7>PJKT SDK</color>: Unable to login to PJKT Services, try restarting unity?");
                auth.SignOut();
            }
        }

        internal static Task RequestProfileUpdate(EditorWindow caller = null)
        {
            return PJKTNet.RequestMessage(PJKTNet.defaultHost + "profile").ContinueWith(task => {
                if (task.IsCanceled) {
                    ErrorResponseHandler("RequestProfileUpdate was canceled.");
                }
                if (task.IsFaulted) {
                    ErrorResponseHandler("RequestProfileUpdate encountered an error: \n" +  task.Exception + " \nDid you enter the correct email and password?");
                }
                //Debug.Log("Profile Response: " + task.Result);
                
                _pjktProfile = JsonUtility.FromJson<PJKTProfile>(task.Result);

                if (caller != null) caller.Repaint();
                //else Debug.Log("Did not repaint");
                return Task.CompletedTask;
            });
        }
        

        //Create a new account from email and password
        internal static Task CreateAccount(string email, string password, string newDisplayName, string newGroupName, EditorWindow caller = null)
        {
            //create account on firebase first
            return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(async task => {
                if (task.IsCanceled) {
                    ErrorResponseHandler("CreateUserWithEmailAndPasswordAsync was canceled.");
                }
                if (task.IsFaulted) {
                    ErrorResponseHandler("CreateUserWithEmailAndPasswordAsync encountered an error:\n" + task.Exception + "\nDid you enter the correct email and password?");
                }

                //set usernames
                UserProfile profile = new UserProfile
                {
                    DisplayName = newDisplayName
                };

                auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWith(async profileTask =>
                {
                    if (profileTask.IsCanceled) {
                        Debug.LogError("UpdateUserProfileAsync was canceled.");
                        return;
                    }
                    if (profileTask.IsFaulted) {
                        Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                        return;
                    }
                });
                displayName = newDisplayName;

                await PJKTLogin();
                
                if (caller != null) caller.Repaint();
                //else Debug.Log("Did not repaint");
            });
        }

        internal static Task ForgotPassword(string email, EditorWindow caller = null)
        {
            return auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
                if (task.IsCanceled) {
                    ErrorResponseHandler("SendPasswordResetEmailAsync was canceled.");
                }
                if (task.IsFaulted) {
                    ErrorResponseHandler("SendPasswordResetEmailAsync encountered an error:\n" + task.Exception + "\nDid you enter the correct email and password?");
                }

                loginMessage = "Password reset email sent to " + email + "\nPlease note that if you have not verified your email address, you will not receive the email";
            });
        }

        private static void ErrorResponseHandler(string error)
        {
            loginMessage = error;
            Debug.LogError("<color=#4557f7>PJKT SDK</color>: " + error);
        }

        internal static void ResetLoginMessage()
        {
            loginMessage = "";
        }

        internal static async void Logout()
        {
            loginMessage = "Logged out";
            auth.SignOut();
            EditorApplication.quitting -= Logout;
           
            PJKTLogoutMessage message = new PJKTLogoutMessage();
            await PJKTNet.SendMessage(message);
            PjktCookie = "";
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Logged out");
        }
    }
}