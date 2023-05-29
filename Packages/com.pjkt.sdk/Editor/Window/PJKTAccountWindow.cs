using UnityEngine;

namespace PJKT.SDK.Window
{
    internal class PJKTAccountWindow : PJKTWindow {
        private string displayNameField = AuthData.displayName;
        private string emailAddressField = AuthData.emailAddress;

        private bool passwordFieldActive = false;
        private string passwordField = "";
        private bool showPasswordField = false;
        
        internal override void OnGUI()
        {
            GUILayout.Label("Display Name");
            displayNameField = GUILayout.TextField(displayNameField);
            if (displayNameField != AuthData.displayName)
            {
                GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save"))
                    {
                        AuthData.displayName = displayNameField;
                    }
                    if (GUILayout.Button("Cancel"))
                    {
                        displayNameField = AuthData.displayName;
                    }
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);

            GUILayout.Label("Email Address");
            emailAddressField = GUILayout.TextField(emailAddressField);
            if (emailAddressField != AuthData.emailAddress)
            {
                GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save"))
                    {
                        AuthData.emailAddress = emailAddressField;
                    }
                    if (GUILayout.Button("Cancel"))
                    {
                        emailAddressField = AuthData.emailAddress;
                    }
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(8);

            GUILayout.Label("Password");
            if (passwordFieldActive)
            {
                passwordField = showPasswordField ? GUILayout.TextField(passwordField) : GUILayout.PasswordField(passwordField, '*');
                GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Save"))
                    {
                        AuthData.password = passwordField;
                        passwordFieldActive = false;
                        passwordField = "";
                        showPasswordField = false;
                    }
                    if (GUILayout.Button("Cancel"))
                    {
                        passwordFieldActive = false;
                        passwordField = "";
                        showPasswordField = false;
                    }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Change Password"))
                {
                    passwordFieldActive = true;
                }
            }

            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Logout", GUILayout.MaxWidth(128)))
                {
                    AuthData.Logout();
                }
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        internal override void OnFocus()
        {
            
        }

        internal override void OnOpen()
        {
            displayNameField = AuthData.displayName;
            emailAddressField = AuthData.emailAddress;
        }
    }
}