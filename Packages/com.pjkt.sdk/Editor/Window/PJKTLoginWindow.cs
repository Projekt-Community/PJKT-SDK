/////////////////////////////////////////////////////////
///                                                   ///
///    Written by Chanoler                            ///
///    If you are a VRChat employee please hire me    ///
///    chanolercreations@gmail.com                    ///
///                                                   ///
/////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace PJKT.SDK.Window
{
    internal class PJKTLoginScreen : PJKTWindow {
        //----------Variables----------//
        private string email = "";
        private string password = "";
        bool showPassword = false;

        private bool createAccountStage2 = false;
        private string newDisplayName = "";
        private string newGroupName = "";

        override internal void OnGUI()
        {
            //Skip if logged in
            if (AuthData.isAuthorized)
            {
                //mainWindow.SwitchTo<PJKTBoothUploaderWindow>();
                mainWindow.SwitchTo<PJKTCustomBoothWindow>();
                return;
            }

            GUILayout.Space(16);
            GUILayout.BeginHorizontal();
                GUILayout.Space(16);
                GUILayout.BeginVertical();
                    //Show the reason for login failure
                    if (AuthData.loginMessage != "")
                    {
                        EditorGUILayout.HelpBox(AuthData.loginMessage, MessageType.Info);
                    }
                    
                    GUILayout.Label("Email Address");
                    email = GUILayout.TextField(email);
                    GUILayout.Label("Password");
                    password = showPassword ? GUILayout.TextField(password) : GUILayout.PasswordField(password, '*');
                    GUILayout.Space(8);
                    showPassword = GUILayout.Toggle(showPassword, "Show password");

                    //No login button if creating an account
                    if (!createAccountStage2)
                    {
                        GUILayout.Space(8);
                        if (GUILayout.Button("Login"))
                        {
                            AuthData.ResetLoginMessage();
                            AuthData.Login(email, password, mainWindow);
                            //Callback is LoginStatusChanged()
                        }
                    }
                    GUILayout.Space(8);
                    //Stage 2 of account creation
                    if (createAccountStage2)
                    {
                        //Ask for a display name and a group name
                        GUILayout.Label("Display Name");
                        newDisplayName = GUILayout.TextField(newDisplayName);
                        GUILayout.Space(8);
                        GUILayout.Label("Group Name");
                        newGroupName = GUILayout.TextField(newGroupName);

                        GUILayout.Space(8);
                        if (GUILayout.Button(new GUIContent("Back to login")))
                        {
                            createAccountStage2 = false;
                        }
                    }
                    GUILayout.Space(8);
                    if (GUILayout.Button("Create account"))
                    {
                        //Begin stage 2 of account creation
                        if (!createAccountStage2)
                        {
                            createAccountStage2 = true;
                        }
                        //Otherwise, create the account
                        else
                        {
                            AuthData.ResetLoginMessage();
                            AuthData.CreateAccount(email, password, newDisplayName, newGroupName, mainWindow);
                            //Callback is LoginStatusChanged()
                        }
                    }


                    GUILayout.Space(16);
                    EditorGUILayout.BeginHorizontal();
                        bool GUIWasEnabled = GUI.enabled;
                        GUI.enabled = email != "";
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Forgot password?"))
                        {
                            AuthData.ResetLoginMessage();
                            AuthData.ForgotPassword(email, mainWindow);
                            //Callback is LoginStatusChanged()
                        }
                        GUI.enabled = GUIWasEnabled;
                        GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    if (email == "")
                    {
                        EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUILayout.Label("Enter your email address first", EditorStyles.centeredGreyMiniLabel);
                            GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }

                    //Destroy the login message if the user changes the credentials
                GUILayout.EndVertical();
                GUILayout.Space(16);
            GUILayout.EndHorizontal();
        }

        internal override void OnFocus()
        {
            
        }

        public void LoginStatusChanged()
        {
            if (AuthData.isAuthorized)
            {
                email = "";
                password = "";
                //mainWindow.SwitchTo<PJKTBoothUploaderWindow>();
                mainWindow.SwitchTo<PJKTCustomBoothWindow>();
            }
            else
            {
                mainWindow.Repaint();
            }
        }
    }
}