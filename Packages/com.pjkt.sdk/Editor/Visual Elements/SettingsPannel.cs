using System.Net;
using System.Net.Http;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SettingsPannel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SettingsPannel>
        {
        }

        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/SettingsPanel.uxml";

        private TextField inviteCodeInput => this.Q<TextField>("PjktTextInput");
        private Button JoinCommunityButton => this.Q<Button>("JoinCommunityButton");
        private Button LogoutButton => this.Q<Button>("LogoutButton");

        public SettingsPannel()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);

            JoinCommunityButton.RegisterCallback<ClickEvent>(JoinCommunity);
            LogoutButton.RegisterCallback<ClickEvent>(Logout);
            
            //if we are in guest mode then change text to exit guest mode and disable join code stuff
            if (!Authentication.IsLoggedIn)
            {
                LogoutButton.text = "Exit Guest Mode";
                LogoutButton.tooltip = "Exit guest mode and log in with your account.";
                
                inviteCodeInput.style.display = DisplayStyle.None;
                JoinCommunityButton.style.display = DisplayStyle.None;
            }
            else
            {
                LogoutButton.text = "Logout";
                LogoutButton.tooltip = "Logout of your account.";
            }
        }

        private void Logout(ClickEvent evt)
        {
            if (Authentication.IsLoggedIn)
            {
                Authentication.Logout();
                return;
            }
            else //guest mode
            {
                PjktSdkWindow window = EditorWindow.GetWindow<PjktSdkWindow>();
                if (window != null) window.ExitGuestMode();
            }
        }

        private async void JoinCommunity(ClickEvent evt)
        {
            if (string.IsNullOrEmpty(inviteCodeInput.value))
            {
                PjktSdkWindow.Notify("You must enter a valid invite code", BoothErrorType.Error);
                return;
            }
            
            PJKTCheckInviteCodeMessage checkInviteCodeMessage = new PJKTCheckInviteCodeMessage(inviteCodeInput.value);
            HttpResponseMessage inviteCodeResponse = await PJKTNet.SendMessage(checkInviteCodeMessage);

            if (inviteCodeResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                //fucked up
                PjktSdkWindow.Notify("Invite code is invalid", BoothErrorType.Error);
                return;
            }

            Authentication.JoinCommunity(inviteCodeInput.value);
        }
    }
}