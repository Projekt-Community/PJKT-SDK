using System.Threading.Tasks;
using PJKT.SDK2.Extras;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class LoginForm : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<LoginForm> { }

        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/LoginPage.uxml";
        
        private Button loginButton => this.Q<Button>("LoginButton");
        private Button registerButton => this.Q<Button>("RegisterButton");
        private Button forgotPasswordButton => this.Q<Button>("ResetPassButton");
        
        //im dumb so now i have to do this
        private TextField emailField => this.Query<TextField>("PjktTextInput").First();
        private TextField passwordField => this.Query<TextField>("PjktTextInput").Last();
        
        public LoginForm()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            loginButton.RegisterCallback<ClickEvent>(Login);
            registerButton.RegisterCallback<ClickEvent>(Register);
            forgotPasswordButton.RegisterCallback<ClickEvent>(ForgotPassword);
            loginButton.style.cursor = SillyCursors.GetSillyCursor();
        }

        private void ForgotPassword(ClickEvent evt)
        {
            if (string.IsNullOrEmpty(emailField.value))
            {
                PjktSdkWindow.Notify("You must enter an email to reset your password.", BoothErrorType.Warning);
                return;
            }
            
            Authentication.ResetPassword(emailField.value);
        }

#pragma warning disable CS4014
        private void Login(ClickEvent evt) => Login();
        private void Register(ClickEvent evt) => Register();
        #pragma warning restore CS4014
        
        private void Login()
        {
            //Authentication.Logout();
            loginButton.style.backgroundColor = PjktGraphics.GetRandomColor();
#pragma warning disable 4014
            ResetLoginButton();
#pragma warning restore 4014
            if (string.IsNullOrEmpty(emailField.value) || string.IsNullOrEmpty(passwordField.value))
            {
                PjktSdkWindow.Notify("You must enter an email and a password", BoothErrorType.Error);
                return;
            }

#pragma warning disable 4014
            Authentication.Login(emailField.value, passwordField.value);
#pragma warning restore 4014
        }
        
        private async void Register()
        {
            //notify the main window
            PjktSdkWindow window = EditorWindow.GetWindow<PjktSdkWindow>();
            await window.ShowRegister();
        }
        
        private async Task ResetLoginButton()
        {
            await Task.Delay(1000);
            loginButton.style.backgroundColor = new Color(0.345098f, 0.345098f, 0.345098f);
        }
    }
}