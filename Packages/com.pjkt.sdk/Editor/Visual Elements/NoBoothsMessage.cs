using UnityEditor;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class NoBoothsMessage : VisualElement
    {
        public class uxmlFactory : UxmlFactory<NoBoothsMessage> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/NoBoothsMessage.uxml";

        public NoBoothsMessage()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
        }
        
        public void SetMessage(string message)
        {
            this.Q<Label>().text = message;
        }
    }
}