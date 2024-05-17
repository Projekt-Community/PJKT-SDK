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

        private const string uxmlPath = "Assets/PJKT SDK 2/Editor/Visual Elements/SettingsPanel.uxml";

        private TextField inviteCodeInput => this.Q<TextField>("PjktTextInput");
        private Button JoinCommunityButton => this.Q<Button>("JoinCommunityButton");
        private Button LogoutButton => this.Q<Button>("LogoutButton");

        public SettingsPannel()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);

            JoinCommunityButton.RegisterCallback<ClickEvent>(JoinCommunity);
            LogoutButton.RegisterCallback<ClickEvent>(Logout);
        }

        private void Logout(ClickEvent evt)
        {
            Authentication.Logout();
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