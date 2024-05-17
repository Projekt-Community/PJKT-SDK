using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class BoothError : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<BoothError> { }
        public BoothError() { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/BoothError.uxml";
        
        private VisualElement errorContainer => this.Q<VisualElement>("Error_Message");
        private VisualElement errorIcon => this.Q<VisualElement>("Icon");
        private Label errorText => this.Q<Label>("Error_Text");
        
        public BoothError(string error, BoothErrorType errorType)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            errorText.text = error;

            switch (errorType)
            {
                case BoothErrorType.Info:
                    errorIcon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("d__Help@2x").image;
                    errorContainer.style.borderBottomColor = new Color(.3f, .3f, .7f);
                    break;
                case BoothErrorType.Warning:
                    errorIcon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.warnicon").image;
                    errorContainer.style.borderBottomColor = new Color(.85f, .85f, .21f);
                    break;
                case BoothErrorType.Error:
                    errorIcon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.erroricon").image;
                    errorContainer.style.borderBottomColor = new Color(1, 0.4f, 0.4f);
                    break;
            }
        }
    }
    
    public enum BoothErrorType
    {
        Info,
        Warning,
        Error,
    }
}