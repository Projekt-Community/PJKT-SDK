using PJKT.SDK2.Extras;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class BoothInfoButton : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BoothInfoButton> { }
        public BoothInfoButton() { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/BoothInfoButton.uxml";
        
        private Label boothName => this.Q<Label>("Booth_Name");
        private Label boothDetails => this.Q<Label>("Information");
        private VisualElement boothSelectButton => this.Q<VisualElement>("Booth_Info_Button");
        private VisualElement boothOptions => this.Q<VisualElement>("Booth_Options");
        private VisualElement boothPreview => this.Q<VisualElement>("PreviewImage");
        private VisualElement boothIssues => this.Q<VisualElement>("Booth_Issues");
        private Button uploadButton => this.Q<Button>("Upload_Button");
        private Button assessButton => this.Q<Button>("AssessBoothButton");
        private Button buildTestButton => this.Q<Button>("BuildTest_Button"); 

        public BoothDescriptor booth { get; private set; }

        public BoothInfoButton(BoothDescriptor boothDescriptor)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            booth = boothDescriptor;
            boothName.text = boothDescriptor.boothName;
            boothOptions.style.display = DisplayStyle.None;
            boothPreview.style.backgroundImage = AssetPreview.GetAssetPreview(boothDescriptor.gameObject);
            
            uploadButton.RegisterCallback<ClickEvent>(UploadBooth);
            assessButton.RegisterCallback<ClickEvent>(CheckBooth);
            buildTestButton.RegisterCallback<ClickEvent>(BuildAndTestBooth);

            StyleCursor cursor = SillyCursors.GetSillyCursor();
            uploadButton.style.cursor = cursor;
            assessButton.style.cursor = cursor;
            buildTestButton.style.cursor = cursor;
        }

        private void CheckBooth(ClickEvent evt)
        {
            boothIssues.Clear();
            SelectBooth(); //may not wanna do this
        }
        
        public void SelectBooth()
        {
            //create report
            BoothValidator.GenerateReport(booth); // not async?

            BoothStats bounds = SDK2.BoothValidator.Report.GetStats(StatsType.Bounds);
            BoothStats vram = SDK2.BoothValidator.Report.GetStats(StatsType.Vram);
            BoothStats fileSize = SDK2.BoothValidator.Report.GetStats(StatsType.FileSize);
            
            //fill in details and show options
            boothDetails.text = bounds.DetailsString + "\n" +
                                vram.DetailsString + "\n" +
                                fileSize.DetailsString;
            boothOptions.style.display = DisplayStyle.Flex;
            assessButton.style.display = DisplayStyle.Flex;
            boothSelectButton.style.maxWidth = StyleKeyword.None;
            boothSelectButton.style.minWidth = 250;
            boothSelectButton.style.flexGrow = 1;
            
            if (PjktEventManager.SelectedProjekt == null)
            {
                uploadButton.SetEnabled(false);
                uploadButton.text = "Select an event to upload";
                boothIssues.Add(new BoothError("Select the event you want to upload for first.", BoothErrorType.Warning));
                return;
            }

            //make error message for stuff over the performance limits
            foreach (BoothStats stats in BoothValidator.Report.Stats)
            {
                if (stats.PerformanceRank < BoothPerformanceRanking.Ok)
                {
                    boothIssues.Add(new BoothError(stats.DetailsString, BoothErrorType.Error));
                }
            }
            
            if (BoothValidator.Report.Overallranking < BoothPerformanceRanking.Ok)
            {
                uploadButton.SetEnabled(false);
                uploadButton.text = "Fix errors before uploading";
            }
        }
        
        public void DeselectBooth()
        {
            boothOptions.style.display = DisplayStyle.None;
            assessButton.style.display = DisplayStyle.None;
            boothSelectButton.style.maxWidth = 112;
            boothSelectButton.style.minWidth = 112;
            style.flexGrow = 0;
            boothIssues.Clear();
        }

        private async void BuildAndTestBooth(ClickEvent evt)
        {
            //check if user is logged into vrcsdk
            if (!VRC.Core.APIUser.IsLoggedIn)
            {
                PjktSdkWindow.Notify("Please Login to the VRChat SDK first!", BoothErrorType.Error);
                return;
            }

            //first dialog box to tell user were gonna make some changes to thier booth
            if (!ConfirmBoothChanges()) return;

            //if accepted run the booth validator prepare booth to mark static change lights and such
            BoothValidator.PrepareBooth(booth);

            //do a test build
            PjktTestScene testScene = new PjktTestScene(booth, new PjktTestSceneOptions()); //options can come from project later
            bool success = await testScene.BuildAndTestBooth();
            
            //refresh booths page
            PjktSdkWindow window = EditorWindow.GetWindow<PjktSdkWindow>();
            window.RefreshPage();
        }

        private async void UploadBooth(ClickEvent evt)
        {
            //booth upload goes here
            if (!uploadButton.enabledSelf) return;
            //check if the uploader is uploading another booth already here
            
            //colors
            Color backgroundColor = PjktGraphics.GetRandomColor();
            Color boarderColor = backgroundColor * .6f;
            
            uploadButton.style.backgroundColor = new StyleColor(backgroundColor);
            uploadButton.style.borderBottomColor = new StyleColor(boarderColor);
            uploadButton.style.borderTopColor = new StyleColor(boarderColor);
            uploadButton.style.borderLeftColor = new StyleColor(boarderColor);
            uploadButton.style.borderRightColor = new StyleColor(boarderColor);
            uploadButton.text = "Uploading...";
            uploadButton.SetEnabled(false);
            
            //check if user is a booth mananger or community manager for the selected community
            bool allowed = false;
            foreach (var community in Authentication.ActiveUser.communityMemberships)
            {
                if (community.community.name != booth.currentCommunity) continue;
                
                foreach (var role in community.community.roles)
                {
                    if (role == "Booth Manager" || role == "Representitive")
                    {
                        allowed = true;
                        break;
                    }
                }
            }

            if (!allowed)
            {
                PjktSdkWindow.Notify($"You are not allowed to upload a booth for {booth.currentCommunity}. Please contact the representative of your community.", BoothErrorType.Error);
                ResetUploadButton();
                return;
            }

            //double check the booth
            BoothValidator.GenerateReport(booth);
            if (BoothValidator.Report.Overallranking < BoothPerformanceRanking.Ok)
            {
                PjktSdkWindow.Notify("Please fix the errors before uploading", BoothErrorType.Error);
                ResetUploadButton();
                return;
            }
            
            //prepare the booth
            if (!ConfirmBoothChanges())
            {
                ResetUploadButton();
                return;
            }
            BoothValidator.PrepareBooth(booth);
            
            //everything checks out
            await BoothUploader.UploadBoothAsync(booth);
            
            //after uploading 
            ResetUploadButton();
        }
        
        private void ResetUploadButton()
        {
            uploadButton.style.backgroundColor = new StyleColor(new Color(0.03529412f, 0.7058824f, 0.5686275f));
            uploadButton.style.borderBottomColor = new StyleColor(new Color(0.02352941f, 0.4666667f, 0.3764706f));
            uploadButton.style.borderTopColor = new StyleColor(new Color(0.02352941f, 0.4666667f, 0.3764706f));
            uploadButton.style.borderLeftColor = new StyleColor(new Color(0.02352941f, 0.4666667f, 0.3764706f));
            uploadButton.style.borderRightColor = new StyleColor(new Color(0.02352941f, 0.4666667f, 0.3764706f));
            uploadButton.text = "Build and upload Booth";
            uploadButton.SetEnabled(true);
        }

        private bool ConfirmBoothChanges()
        {
            string message = $"This will make the following changes to your booth: " +
                             $"\nAll Lights will be set to baked" +
                             $"\nAll lights will have their range and intensity limited" +
                             $"\nAll non animated objects will have their static flags adjusted" +
                             $"\n\nIf there is any unusual behaviour after this process please let us know on the discord.";
            if (!EditorUtility.DisplayDialog("Confirm changes", message, "Go for it", "Actually, hold up"))
            {
                PjktSdkWindow.Notify("Build canceled", BoothErrorType.Warning);
                return false;
            }

            return true;
        }
    }
}