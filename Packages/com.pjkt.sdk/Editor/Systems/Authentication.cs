using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                OnLoginStatusChanged(null, EventArgs.Empty);
            }
        }

        public static event EventHandler OnLoginStatusChanged = delegate { };

        // Firebase REST API constants
        private const string FirebaseApiKey = "AIzaSyBZ5WdA9BbZUMAtlKxKKN-8579f2_AQucI";
        private const string FirebaseSignUpEndpoint = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
        private const string FirebaseSignInEndpoint = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";
        private const string FirebaseSendOobCodeEndpoint = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=";

        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly string accountDataPath = Application.persistentDataPath + "/Projekt Community/UnityEditor/";

        internal static string PjktCookie = "";
        internal static bool isAuthorized { get { return ActiveUser != null && !string.IsNullOrEmpty(PjktCookie); } } // Auth Check
        internal static string token { get; private set; } // This will be the Firebase ID token

        // Static constructor: Removed Firebase SDK initialization
        static Authentication()
        {
            // Just here, to follow firebase SDK pattern stuff
        }

        #region Firebase REST API Helper Classes
        [System.Serializable]
        private class FirebaseSignUpRequest
        {
            public string email;
            public string password;
            public bool returnSecureToken = true;
        }

        [System.Serializable]
        private class FirebaseSignInPasswordRequest
        {
            public string email;
            public string password;
            public bool returnSecureToken = true;
        }

        [System.Serializable]
        private class FirebaseTokenResponse
        {
            public string kind;
            public string idToken; // Firebase ID token
            public string email;
            public string refreshToken;
            public string expiresIn;
            public string localId; // User UID
            public bool registered; 
        }

        [System.Serializable]
        private class FirebaseSendOobCodeRequest
        {
            public string requestType;
            public string email;
        }
        
        [System.Serializable]
        private class FirebaseErrorContainer
        {
            public FirebaseError error;
        }

        [System.Serializable]
        private class FirebaseError
        {
            public int code;
            public string message;
            // public List<FirebaseErrorDetail> errors;
        }
        /*
        [System.Serializable]
        private class FirebaseErrorDetail
        {
            public string message;
            public string domain;
            public string reason;
        }
        */
        #endregion

        private static async Task<string> ParseFirebaseError(HttpResponseMessage response)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            try
            {
                FirebaseErrorContainer errorContainer = JsonUtility.FromJson<FirebaseErrorContainer>(errorContent);
                if (errorContainer != null && errorContainer.error != null)
                {
                    return $"Firebase Error: {errorContainer.error.message} (Code: {errorContainer.error.code})";
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse Firebase error JSON: {ex.Message}. Raw error: {errorContent}");
            }
            return $"HTTP Error {response.StatusCode}. Details: {errorContent}";
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
            if (PjktCookie == "") return false;
            string response = await PJKTNet.RequestMessage("/me");

            if (string.IsNullOrEmpty(response)) return false;
            if (response.Contains("Unauthorized! Verification Failed")) return false;

            try { ActiveUser = JsonUtility.FromJson<PjktActiveUser>(response); }
            catch { return false; }

            return ActiveUser != null;
        }

        internal static async Task Register(string userName, string email, string password, string inviteCode)
        {
            FirebaseSignUpRequest requestPayload = new FirebaseSignUpRequest { email = email, password = password };
            string jsonPayload = JsonUtility.ToJson(requestPayload);
            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(FirebaseSignUpEndpoint + FirebaseApiKey, content);
            }
            catch (HttpRequestException e)
            {
                PjktSdkWindow.Notify($"Register failed due to network issue: {e.Message}", BoothErrorType.Error);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await ParseFirebaseError(response);
                PjktSdkWindow.Notify($"Register failed: {errorMsg}", BoothErrorType.Error);
                return;
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            FirebaseTokenResponse tokenResponse = JsonUtility.FromJson<FirebaseTokenResponse>(responseJson);
            
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.idToken))
            {
                PjktSdkWindow.Notify("Register failed: Could not get token from Firebase.", BoothErrorType.Error);
                return;
            }
            token = tokenResponse.idToken;

            // Create PJKT account
            PJKTAccountCreationMessage accountCreationMessage = new PJKTAccountCreationMessage(userName, inviteCode, token);
            HttpResponseMessage accountCreationResponse = await PJKTNet.SendMessage(accountCreationMessage);

            if (accountCreationResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IEnumerable<string> cookieHeaders;
                if (accountCreationResponse.Headers.TryGetValues("set-cookie", out cookieHeaders))
                {
                    string[] headerdata = cookieHeaders.FirstOrDefault().Split(';');
                    string cookie = headerdata[0];
                    PjktCookie = cookie.Substring(8);
                }
                else
                {
                    PjktSdkWindow.Notify("Unable to create PJKT account: Missing session cookie.", BoothErrorType.Error);
                    // Potentially sign out/clear Firebase token if partial registration is an issue
                    return;
                }

                PjktSessionInfo session = new PjktSessionInfo(PjktCookie);
                string savedSessionData = JsonUtility.ToJson(session);
                if (!Directory.Exists(accountDataPath)) Directory.CreateDirectory(accountDataPath);
                File.WriteAllText(accountDataPath + "SessionData.pjkt", savedSessionData);

                bool foundProfile = await GetActiveUserInfo();
                if (!foundProfile)
                {
                    PjktSdkWindow.Notify("Account created, but error getting profile. Try logging in.", BoothErrorType.Warning);
                    Logout(); // Clears local session data
                    return;
                }
                PjktSdkWindow.Notify("Account created successfully!", BoothErrorType.Info);
                IsLoggedIn = true;
            }
            else
            {
                 string pjktError = await accountCreationResponse.Content.ReadAsStringAsync();
                 PjktSdkWindow.Notify($"Failed to create PJKT account: {accountCreationResponse.ReasonPhrase} - {pjktError}", BoothErrorType.Error);
            }
        }

        internal static async Task Login(string email, string password)
        {
            FirebaseSignInPasswordRequest requestPayload = new FirebaseSignInPasswordRequest { email = email, password = password };
            string jsonPayload = JsonUtility.ToJson(requestPayload);
            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(FirebaseSignInEndpoint + FirebaseApiKey, content);
            }
            catch (HttpRequestException e)
            {
                PjktSdkWindow.Notify($"Login failed due to network issue: {e.Message}", BoothErrorType.Error);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await ParseFirebaseError(response);
                PjktSdkWindow.Notify($"Login failed: {errorMsg}", BoothErrorType.Error);
                return;
            }
            
            string responseJson = await response.Content.ReadAsStringAsync();
            FirebaseTokenResponse tokenResponse = JsonUtility.FromJson<FirebaseTokenResponse>(responseJson);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.idToken))
            {
                PjktSdkWindow.Notify("Login failed: Could not get token from Firebase.", BoothErrorType.Error);
                return;
            }
            token = tokenResponse.idToken; // Store Firebase ID token

            // Create PJKT login message for cookie stuff
            PJKTLoginMessage message = new PJKTLoginMessage(); 
            HttpResponseMessage pjktNetResponse = await PJKTNet.SendMessage(message);

            IEnumerable<string> cookieHeaders;
            if (pjktNetResponse.IsSuccessStatusCode && pjktNetResponse.Headers.TryGetValues("set-cookie", out cookieHeaders))
            {
                string[] headerdata = cookieHeaders.FirstOrDefault().Split(';');
                string cookie = headerdata[0];
                PjktCookie = cookie.Substring(8);
            }
            else
            {
                string pjktError = await pjktNetResponse.Content.ReadAsStringAsync();
                PjktSdkWindow.Notify($"Unable to login to PJKT Services: {pjktNetResponse.ReasonPhrase} - {pjktError}", BoothErrorType.Error);
                token = null;
                return;
            }
            
            PjktSessionInfo session = new PjktSessionInfo(PjktCookie);
            string savedSessionData = JsonUtility.ToJson(session);
            if (!Directory.Exists(accountDataPath)) Directory.CreateDirectory(accountDataPath);
            File.WriteAllText(accountDataPath + "SessionData.pjkt", savedSessionData);

            bool foundProfile = await GetActiveUserInfo();
            if (!foundProfile)
            {
                PjktSdkWindow.Notify("Logged in, but error getting profile info. Try again or relog.", BoothErrorType.Warning);
                Logout();
                return;
            }
            
            IsLoggedIn = true;
            PjktSdkWindow.Notify("Login successful!", BoothErrorType.Info); // Tada! (maybe)
        }

        public static async void Logout()
        {
            
            token = null;

            // Call PJKT API to log out
            PJKTLogoutMessage message = new PJKTLogoutMessage();
            try 
            {
                await PJKTNet.SendMessage(message);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error during PJKT logout: {e.Message}");
            }


            if (File.Exists(accountDataPath + "SessionData.pjkt"))
            {
                File.Delete(accountDataPath + "SessionData.pjkt");
            }
            PjktCookie = "";
            ActiveUser = null; // Clear active user
            IsLoggedIn = false;
            PjktSdkWindow.Notify("Logged out.", BoothErrorType.Info);
        }

        public static async void ResetPassword(string email)
        {
            FirebaseSendOobCodeRequest requestPayload = new FirebaseSendOobCodeRequest { requestType = "PASSWORD_RESET", email = email };
            string jsonPayload = JsonUtility.ToJson(requestPayload);
            StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(FirebaseSendOobCodeEndpoint + FirebaseApiKey, content);
            }
            catch (HttpRequestException e)
            {
                PjktSdkWindow.Notify($"Password reset failed due to network issue: {e.Message}", BoothErrorType.Error);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = await ParseFirebaseError(response);
                PjktSdkWindow.Notify($"Password reset failed: {errorMsg}", BoothErrorType.Error);
                return;
            }
            
            PjktSdkWindow.Notify("Password reset email sent. Please check your inbox.", BoothErrorType.Info);
        }

        public static async void JoinCommunity(string inviteCode)
        {
            PJKTJoinCommunityMessage joinCommunityMessage = new PJKTJoinCommunityMessage(inviteCode);
            HttpResponseMessage response = await PJKTNet.SendMessage(joinCommunityMessage); 

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                PjktSdkWindow.Notify($"Unable to join community: {response.ReasonPhrase} - {errorBody}", BoothErrorType.Error);
                return;
            }

            bool refreshUser = await GetActiveUserInfo();
            if (!refreshUser)
            {
                PjktSdkWindow.Notify("Community joined, but unable to refresh user profile. Try logging out and back in.", BoothErrorType.Warning);
                return;
            }
            
            PjktSdkWindow.Notify("Community joined successfully!", BoothErrorType.Info);
        }
    }

    [Serializable]
    internal class PjktSessionInfo
    {
        public string SessionToken; // This is the muchy cookie content

        public PjktSessionInfo(string sessionToken)
        {
            SessionToken = sessionToken;
        }
    }
}