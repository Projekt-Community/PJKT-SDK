using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class RequirementsInfo : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<RequirementsInfo> { }
        private const string uxmlPath = "Assets/PJKT SDK 2/Editor/Visual Elements/RequirementsInfo.uxml";
        public RequirementsInfo() { }
        public RequirementsInfo(BoothStats[] boothstats)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            string boothInfo = "";
            
            if (BoothValidator.SelectedBooth == null)
            {
                boothInfo = "You must select a booth \nto view requirements";
                SetBoothInfo(boothInfo);
                return;
            }
            
            foreach (BoothStats stats in boothstats)
            {
                boothInfo += stats.DetailsString + "\n";
            }
            
            string requirementsInfo = "";
            foreach (BoothStats stats in boothstats)
            {
                requirementsInfo += stats.RequirementsString + "\n";
            }
            
            SetEventInfo(requirementsInfo);
            SetBoothInfo(boothInfo);
        }

        private Label eventName => this.Q<Label>("Event_Name");
        private Label eventRequirements => this.Q<Label>("Event_Requirements");
        private Label boothName => this.Q<Label>("Booth_Name");
        private Label boothInfo => this.Q<Label>("Booth_Info");

        private void SetEventInfo(string requirements)
        {
            if (PjktEventManager.SelectedProjekt == null)
            {
                eventName.text = "You must select an event \nto view requirements";
                return;
            }
            eventName.text = "Requirements for: \n" + PjktEventManager.SelectedProjekt.name;
            eventRequirements.text = requirements;
        }
        private void SetBoothInfo(string boothStats)
        {
            if (BoothValidator.SelectedBooth == null)
            {
                boothName.text = "You must select a booth \nto view requirements";
                return;
            }
            boothName.text = BoothValidator.SelectedBooth.boothName;
            boothInfo.text = boothStats;
        }
    }
}