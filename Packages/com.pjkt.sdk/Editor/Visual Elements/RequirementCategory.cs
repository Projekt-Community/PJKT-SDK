using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class RequirementCategory : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<RequirementCategory> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/RequirementCategory.uxml";
        public RequirementCategory() { }
        
        private VisualElement performanceIcon => this.Q<VisualElement>("Performance_Icon");
        private Label requirementText => this.Q<Label>("Requirement_Text");
        
        public RequirementCategory(string requirement, Texture2D icon)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);

            performanceIcon.style.backgroundImage = icon;
            requirementText.text = requirement;
        }
    }
}