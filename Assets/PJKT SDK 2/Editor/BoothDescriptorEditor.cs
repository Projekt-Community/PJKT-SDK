using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{  
    [CustomEditor(typeof(BoothDescriptor))]
    public class PJKTBoothDescriptorEditor : Editor
    {
        private const string uxmlPath = "Assets/PJKT SDK 2/Editor/Visual Elements/BoothDescriptor.uxml";

        private Label boothName;
        private BoothDescriptor descriptor;

        //bounds option
        private VisualElement boundsToggle;
        private VisualElement boundsCheck;
        
        //group id
        private TextField groupID;
        
        //community options
        private Label currentCommunity;
        private VisualElement communityOptions;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            var clone = asset.Instantiate();
            
             descriptor = target as BoothDescriptor;


            boothName = clone.Q<Label>("BoothName");
            boothName.text = target.name;
            
            boundsToggle = clone.Q<VisualElement>("ShowBoundsToggle");
            boundsCheck = clone.Q<VisualElement>("BoundsCheck");
            boundsToggle.RegisterCallback<ClickEvent>(ShowBounds);
            SerializedProperty showBounds = serializedObject.FindProperty("showBounds");
            boundsCheck.style.display = showBounds.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            
            groupID = clone.Q<TextField>("Group_ID_Input");
            groupID.RegisterValueChangedCallback(UpdateGroupID);
            groupID.value = serializedObject.FindProperty("GroupID").stringValue;
            
            currentCommunity = clone.Q<Label>("CurrentOption");
            currentCommunity.text = serializedObject.FindProperty("currentCommunity").stringValue;
            
            communityOptions = clone.Q<VisualElement>("CommunityOptions");
            currentCommunity.RegisterCallback<ClickEvent>(ShowCommunities);
            communityOptions.RegisterCallback<MouseLeaveEvent>(HideCommunityOptions);
            
            FillCommunities();
            
            return clone;
        }
        
        private void UpdateGroupID(ChangeEvent<string> evt)
        {
            serializedObject.FindProperty("GroupID").stringValue = groupID.value;
            serializedObject.ApplyModifiedProperties();
        }

        private void FillCommunities()
        {
            if (Authentication.ActiveUser == null) return;
            
            //destroy all children in the community options
            communityOptions.Clear();
            
            //clone the current community option for each community in the list
            foreach (var community in Authentication.ActiveUser.communityMemberships)
            {
                Label communityOption = new Label(community.community.name);
                communityOption.style.unityFontStyleAndWeight = FontStyle.Bold;

                communityOption.RegisterCallback<ClickEvent>(ChangeCommunity);
                communityOptions.Add(communityOption);
            }
        }
        
        private void ShowBounds(ClickEvent evt)
        {
            serializedObject.Update();
            bool showBounds = !serializedObject.FindProperty("showBounds").boolValue;
            serializedObject.FindProperty("showBounds").boolValue = showBounds;
            serializedObject.ApplyModifiedProperties();
            
            boundsCheck.style.display = showBounds ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void ShowCommunities(ClickEvent evt)
        {
            if (communityOptions.childCount == 0) return;
            communityOptions.style.display = DisplayStyle.Flex;
        }

        private void HideCommunityOptions(MouseLeaveEvent evt)
        {
            communityOptions.style.display = DisplayStyle.None;
        }
        
        private void ChangeCommunity(ClickEvent evt)
        {
            Label label = evt.target as Label;
            if (label == null) return;
            serializedObject.FindProperty("currentCommunity").stringValue = label.text;
            serializedObject.ApplyModifiedProperties();
            currentCommunity.text = label.text;
            communityOptions.style.display = DisplayStyle.None;
        }
    }
}
