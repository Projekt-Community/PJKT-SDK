using System.Collections.Generic;
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
        private Button exportButton => this.Q<Button>("AssessBoothButton");
        //private Button buildTestButton => this.Q<Button>("BuildTest_Button"); 
        
        //community options
        private Label currentCommunity => this.Q<Label>("CurrentOption");
        private VisualElement communityOptions => this.Q<VisualElement>("CommunityOptions");
        private List<string> communityNames = new List<string>();

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
            exportButton.RegisterCallback<ClickEvent>(ExportBoothFiles);
            //buildTestButton.RegisterCallback<ClickEvent>(BuildAndTestBooth);

            StyleCursor cursor = SillyCursors.GetSillyCursor();
            uploadButton.style.cursor = cursor;
            exportButton.style.cursor = cursor;
            //buildTestButton.style.cursor = cursor;
            
            currentCommunity.RegisterCallback<ClickEvent>(ShowCommunities);
            currentCommunity.text = booth.currentCommunity;
            
            communityOptions.RegisterCallback<MouseLeaveEvent>(HideCommunityOptions);
            FillCommunities();
        }

        
        public void CheckBooth()
        {
            boothIssues.Clear();
            SelectBooth(); //may not wanna do this
        }
        
        private void FillCommunities()
        {
            if (Authentication.ActiveUser == null) return;
            
            //destroy all children in the community options
            communityOptions.Clear();
            communityNames.Clear();
            
            //clone the current community option for each community in the list
            foreach (var community in Authentication.ActiveUser.communityMemberships)
            {
                Label communityOption = new Label(community.community.name);
                communityOption.style.unityFontStyleAndWeight = FontStyle.Bold;
                communityOption.style.color = Color.white;

                communityOption.RegisterCallback<ClickEvent>(ChangeCommunity);
                communityOption.RegisterCallback<MouseEnterEvent>(HighlightOption);
                communityOption.RegisterCallback<MouseLeaveEvent>(UnhighlightOption);
                communityOptions.Add(communityOption);
                communityNames.Add(community.community.name);
            }
        }
        
        private void ShowCommunities(ClickEvent evt)
        {
            if (communityOptions.childCount == 0) return;
            communityOptions.style.display = DisplayStyle.Flex;
        }

        private void HideCommunityOptions(MouseLeaveEvent evt)
        {
            communityOptions.style.display = DisplayStyle.None;
        }
        
        private void ChangeCommunity(ClickEvent evt)
        {
            Label label = evt.target as Label;
            if (label == null) return;
            
            SerializedObject so = new SerializedObject(booth);
            so.FindProperty("currentCommunity").stringValue = label.text;
            so.ApplyModifiedProperties();
            currentCommunity.text = label.text;
            communityOptions.style.display = DisplayStyle.None;
        }

        private void HighlightOption(MouseEnterEvent evt)
        {
            Label label = evt.target as Label;
            if (label == null) return;
            
            label.style.backgroundColor = new StyleColor(new Color(0f, 0.4f, 0.6f, 0.8f));
        }
        
        private void UnhighlightOption(MouseLeaveEvent evt)
        {
            Label label = evt.target as Label;
            if (label == null) return;
            
            label.style.backgroundColor = new StyleColor(Color.clear);
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
            exportButton.style.display = DisplayStyle.Flex;
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
            
            //make sure a community is selected first
            if (string.IsNullOrEmpty(booth.currentCommunity))
            {
                uploadButton.SetEnabled(false);
                uploadButton.text = "Select a community to upload for";
                boothIssues.Add(new BoothError("Please select a community for this booth in the Booth Descriptor component.", BoothErrorType.Warning));
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
            exportButton.style.display = DisplayStyle.None;
            boothSelectButton.style.maxWidth = 112;
            boothSelectButton.style.minWidth = 112;
            style.flexGrow = 0;
            boothIssues.Clear();
        }

        //getting rid of this
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

        //basically creates the zip and stops there
        private void ExportBoothFiles(ClickEvent evt)
        {
            string path = EditorUtility.SaveFilePanel("Export Booth", "", booth.currentCommunity + ".zip", "zip");
            if (string.IsNullOrEmpty(path)) return;

            if (!ConfirmBoothChanges()) return;
            BoothValidator.PrepareBooth(booth);
            
            PjktFileExporter exporter = new PjktFileExporter(booth.currentCommunity, path);
            string packagePath = exporter.CreateBoothfile(booth.gameObject);
            
            if (string.IsNullOrEmpty(packagePath))
            {
                //failed to create zip for some reason
                PjktSdkWindow.Notify($"Failed to create booth package. Ask for help on the discord.", BoothErrorType.Error);
                return;
            }
            
                PjktSdkWindow.Notify($"Booth exported to {packagePath}");
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
            
            //copyright disclaimer
            if (!EditorUtility.DisplayDialog("Copyright Disclaimer", "By uploading this booth you confirm that you have the rights to all the content in this booth and that you are allowed to upload it to VRChat. If you do not have the rights to any of the content in this booth please do not upload it. If you are unsure about the rights of any content in your booth please ask for help on the discord. By uploading this booth you hereby give Projekt: Community a licence to use the uploaded content within our VRChat worlds for the event, as well as permission to appear in our media and promotional content.", "I have the rights", "Cancel Upload"))
            {
                PjktSdkWindow.Notify("Upload canceled", BoothErrorType.Warning);
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
                             $"\nAny directional lights will be removed" +
                             $"\nAll non animated objects will have their static flags adjusted" +
                             $"\nAll reflection probes will be removed" +
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