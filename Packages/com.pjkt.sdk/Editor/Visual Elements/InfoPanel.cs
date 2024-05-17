using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class InfoPanel : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InfoPanel> { }
        public InfoPanel() { }
    
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/InfoPanel.uxml";
        private Button SelectButton => this.Q<Button>("SelectObjectButton");
        private Label InfoLabel => this.Q<Label>("InfoText");
        private VisualElement typeIcon => this.Q<VisualElement>("TypeIcon");
    
        private Object refrenceObject;


        public InfoPanel(Object obj, Texture2D icon, string assetType, string info, Color color)
        {
            Setup(obj, icon, assetType, info, color);
        }

        private void Setup(Object obj, Texture2D icon, string assetType, string info, Color color)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
        
            //object info
            refrenceObject = obj;
            InfoLabel.text = info;
            SelectButton.text = obj.name;
            
            //type icon
            typeIcon.style.backgroundImage = icon;
            typeIcon.tooltip = assetType;

            //button colors
            SelectButton.style.borderTopColor = color;
            SelectButton.style.borderLeftColor = color;
            SelectButton.style.borderRightColor = color;
            SelectButton.style.borderBottomColor = color;
            SelectButton.RegisterCallback<ClickEvent>(SelectObject);
            SelectButton.RegisterCallback<MouseEnterEvent>(OnHoverEnter);
            SelectButton.RegisterCallback<MouseLeaveEvent>(OnHoverExit);
        }

        public void AddMessage(VisualElement element)
        {
            SelectButton.Add(element);
        }

        public void AddMessageAtIndex(VisualElement element, int index)
        {
            SelectButton.Insert(index, element);
        }
    
        private void SelectObject(ClickEvent evt)
        {
            if (refrenceObject == null) return;
            EditorGUIUtility.PingObject(refrenceObject);
        }
        
        private void OnHoverEnter(MouseEnterEvent evt)
        {
            SelectButton.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        }
        private void OnHoverExit(MouseLeaveEvent evt)
        {
            SelectButton.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f);
        }
    }
}