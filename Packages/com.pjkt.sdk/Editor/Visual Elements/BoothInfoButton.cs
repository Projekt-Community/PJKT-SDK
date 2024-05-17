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
        private  Button assessButton => this.Q<Button>("AssessBoothButton");

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

            uploadButton.style.cursor = SillyCursors.GetSillyCursor();
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
            
            //make error message for stuff over the performance limits
            foreach (BoothStats stats in BoothValidator.Report.Stats)
            {
                if (stats.PerformanceRank < BoothPerformanceRanking.Ok)
                {
                    boothIssues.Add(new BoothError(stats.DetailsString, BoothErrorType.Error));
                }
            }

            if (PjktEventManager.SelectedProjekt == null)
            {
                uploadButton.SetEnabled(false);
                uploadButton.text = "Select an event to upload";
                return;
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
            uploadButton.text = "Upload Booth";
            uploadButton.SetEnabled(true);
        }
    }
}