using UnityEditor;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SDKPage : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<SDKPage> { }

        private const string uxmlPath = "Assets/PJKT SDK 2/Editor/Visual Elements/SDKPage.uxml";
        protected ScrollView scrollView => this.Q<ScrollView>("PageScroll");
        protected VisualElement topArea => this.Q<VisualElement>("Top_Area");
        
        protected PjktSdkWindow sdkWindow;
        
        public SDKPage()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
        }

        public virtual void OnTabEnable() { }
        public virtual void OnTabDisable() { }
    }
} 