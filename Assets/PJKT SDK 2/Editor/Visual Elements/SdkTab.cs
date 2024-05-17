using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SdkTab : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SdkTab> { }
        public SdkTab() { }
        private const string uxmlPath = "Assets/PJKT SDK 2/Editor/Visual Elements/Tab.uxml";
        private VisualElement tabIcon => this.Q<VisualElement>("TabIcon");
        Button tabButton => this.Q<Button>("TabButton");
        private Color tabColor;
        private Color defaultColor = new Color(0.1882353f, 0.1882353f, 0.1882353f);
        private bool activeTab = false;
        public readonly SDKPageType pageType;
        
        public SdkTab(Texture2D icon, Color color, SDKPageType sdkPage)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            tabIcon.style.backgroundImage = icon;
            tabColor = color;
            pageType = sdkPage;
            
            tabButton.RegisterCallback<MouseEnterEvent>(OnHoverEnter);
            tabButton.RegisterCallback<MouseLeaveEvent>(OnHoverExit);
            
            OnHoverExit(new MouseLeaveEvent());
        }

        public void SelectTab(bool selected)
        {
            activeTab = selected;
            tabButton.style.borderTopColor = selected ? tabColor : defaultColor;
            tabButton.style.borderRightColor = selected ? tabColor : defaultColor;
            tabButton.style.borderBottomColor = selected ? tabColor : defaultColor;
            
            style.translate = selected ? new Translate(0, 0) : new Translate(-10, 0);
                        
        }
        private void OnHoverEnter(MouseEnterEvent evt)
        {
            if (activeTab) return;
            style.translate = new Translate(0, 0);
            Color lerpColor = Color.Lerp(defaultColor, tabColor, 0.5f);
            tabButton.style.borderTopColor = lerpColor;
            tabButton.style.borderRightColor = lerpColor;
            tabButton.style.borderBottomColor = lerpColor;
        }
        
        private void OnHoverExit(MouseLeaveEvent evt)
        {
            if (activeTab) return;
            style.translate = new Translate(-10, 0);
            tabButton.style.borderTopColor = defaultColor;
            tabButton.style.borderRightColor = defaultColor;
            tabButton.style.borderBottomColor = defaultColor;
        }
    }
}