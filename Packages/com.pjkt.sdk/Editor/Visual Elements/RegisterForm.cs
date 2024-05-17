using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#pragma warning disable CS4014
namespace PJKT.SDK2
{
    public class RegisterForm : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<RegisterForm> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/RegisterForm.uxml";
        
        private Button registerButton => this.Q<Button>("RegisterButton");
        private TextField userNameField => this.Query<TextField>("PjktTextInput").AtIndex(0);
        private TextField emailField => this.Query<TextField>("PjktTextInput").AtIndex(1);
        private TextField passwordField => this.Query<TextField>("PjktTextInput").AtIndex(2);
        private TextField inviteCodeField => this.Query<TextField>("PjktTextInput").AtIndex(3);
        public RegisterForm()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);

            registerButton.RegisterCallback<ClickEvent>(Register);
        }
        
        private void Register(ClickEvent evt) => Register();

        private async Task Register()
        {
            registerButton.style.backgroundColor = PjktGraphics.GetRandomColor();
            
            if (string.IsNullOrEmpty(emailField.value) || string.IsNullOrEmpty(passwordField.value) || string.IsNullOrEmpty(userNameField.value) || string.IsNullOrEmpty(inviteCodeField.value))
            {
                PjktSdkWindow.Notify("You must enter something in all fields", BoothErrorType.Error);
                return;
            }
            
            PJKTCheckInviteCodeMessage checkInviteCodeMessage = new PJKTCheckInviteCodeMessage(inviteCodeField.value);
            HttpResponseMessage inviteCodeResponse = await PJKTNet.SendMessage(checkInviteCodeMessage);

            if (inviteCodeResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                //fucked up
                PjktSdkWindow.Notify("Invite code is invalid", BoothErrorType.Error);
                return;
            }
            
            if (inviteCodeResponse.StatusCode == HttpStatusCode.OK)
            {
                //code is good, register with firebase
                Authentication.Register(userNameField.value, emailField.value, passwordField.value, inviteCodeField.value);
            }
        }
    }
}