using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class Notification : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Notification> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/Notification.uxml";
        
        private VisualElement container => this.Q<VisualElement>("Notification");
        private VisualElement icon => this.Q<VisualElement>("Icon");
        private Label text => this.Q<Label>("Error_Text");

        public Notification()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            style.transitionDuration = new List<TimeValue> {new TimeValue(300, TimeUnit.Millisecond)};
        }
        
        public void SetNotification(string error, BoothErrorType errorType)
        {
            text.text = error;

            switch (errorType)
            {
                case BoothErrorType.Info:
                    icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("d__Help@2x").image;
                    container.style.borderBottomColor = new Color(.3f, .3f, .7f);
                    break;
                case BoothErrorType.Warning:
                    icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.warnicon").image;
                    container.style.borderBottomColor = new Color(.85f, .85f, .21f);
                    break;
                case BoothErrorType.Error:
                    icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent("console.erroricon").image;
                    container.style.borderBottomColor = new Color(1, 0.4f, 0.4f);
                    break;
            }
        }
        public void HideNotification()
        {
            style.translate = new Translate(0, 128);
        }
        public void ShowNotification()
        {
            style.translate = new Translate(0, 0);
        }
    }
}