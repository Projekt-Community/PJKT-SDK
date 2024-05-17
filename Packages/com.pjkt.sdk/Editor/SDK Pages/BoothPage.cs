using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class BoothPage : SDKPage
    {
        private BoothInfoButton selectedButton;
        public override void OnTabEnable()
        {
            BoothValidator.GetBoothsInScene();
            
            scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
            scrollView.contentContainer.style.flexWrap = Wrap.Wrap;
            
            if (BoothValidator.BoothsInScene.Length == 0)
            {
                topArea.Add(new NoBoothsMessage());
                return;
            }
            
            RefreshBoothButtons();
        }
        
        public override void OnTabDisable()
        {

        }

        private void RefreshBoothButtons()
        {
            //create an info panel for each booth
            foreach (BoothDescriptor booth in BoothValidator.BoothsInScene)
            {
                BoothInfoButton boothButton = new BoothInfoButton(booth);
                VisualElement button = boothButton.Q<VisualElement>("Booth_Info_Button");
                button.RegisterCallback<ClickEvent>(OnBoothButtonClick);

                scrollView.Add(boothButton);
                
                if (BoothValidator.SelectedBooth == booth)
                {
                    selectedButton = boothButton;
                    selectedButton.SelectBooth();
                    topArea.Add(selectedButton);
                }
            }
        }

        private void OnBoothButtonClick(ClickEvent evt)
        {
            VisualElement button = evt.currentTarget as VisualElement;
            BoothInfoButton boothButton = button.parent as BoothInfoButton;
            if (boothButton == null) return;
            if (selectedButton == boothButton) return; //already selected

            if (selectedButton != null)
            {
                selectedButton.DeselectBooth();
                topArea.Remove(selectedButton);
                scrollView.Add(selectedButton);
            }
            
            selectedButton = boothButton;
            selectedButton.SelectBooth();
            
            topArea.Add(selectedButton);
        }
    }
}