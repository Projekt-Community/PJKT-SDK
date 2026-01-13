using System.Collections.Generic;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{  
    [CustomEditor(typeof(BoothDescriptor))]
    public class PJKTBoothDescriptorEditor : Editor
    {
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/BoothDescriptor.uxml";

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
        private List<string> communityNames = new List<string>();
        
        //reps
        private TextField rep1;
        private TextField rep2;
        private TextField rep3;
        
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
            
            rep1 = clone.Q<TextField>("Rep1_Input");
            rep1.RegisterValueChangedCallback(evt => UpdateRepresentitives(evt, 0));
            rep1.value = serializedObject.FindProperty("representitives").GetArrayElementAtIndex(0).stringValue;
            rep2 = clone.Q<TextField>("Rep2_Input");
            rep2.RegisterValueChangedCallback(evt => UpdateRepresentitives(evt, 1));
            rep2.value = serializedObject.FindProperty("representitives").GetArrayElementAtIndex(1).stringValue;
            rep3 = clone.Q<TextField>("Rep3_Input");
            rep3.RegisterValueChangedCallback(evt => UpdateRepresentitives(evt, 2));
            rep3.value = serializedObject.FindProperty("representitives").GetArrayElementAtIndex(2).stringValue;
            
            currentCommunity = clone.Q<Label>("CurrentOption");
            SerializedProperty currentCommunityProp = serializedObject.FindProperty("currentCommunity");
            currentCommunity.text = currentCommunityProp.stringValue;
            
            communityOptions = clone.Q<VisualElement>("CommunityOptions");
            currentCommunity.RegisterCallback<ClickEvent>(ShowCommunities);
            communityOptions.RegisterCallback<MouseLeaveEvent>(HideCommunityOptions);
            
            FillCommunities();
            if (string.IsNullOrEmpty(currentCommunityProp.stringValue))
            {
                //try to autofill with first item in list
                if (communityNames.Count > 0)
                {
                    currentCommunity.text = communityNames[0];
                    currentCommunityProp.stringValue = communityNames[0];
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
            return clone;
        }
        
        private void UpdateGroupID(ChangeEvent<string> evt)
        {
            serializedObject.FindProperty("GroupID").stringValue = groupID.value;
            serializedObject.ApplyModifiedProperties();
        }
        
        private void UpdateRepresentitives(ChangeEvent<string> evt, int repNumber)
        {
            SerializedProperty repsProp = serializedObject.FindProperty("representitives");
            if (repNumber < 0 || repNumber >= repsProp.arraySize) return;
            repsProp.GetArrayElementAtIndex(repNumber).stringValue = evt.newValue;
            serializedObject.ApplyModifiedProperties();
        }

        private void FillCommunities()
        {
            if (Authentication.ActiveUser == null) return;
            
            //destroy all children in the community options
            communityOptions.Clear();
            communityNames.Clear();
            
            //clone the current community option for each community in the list
            foreach (var community in Authentication.ActiveUser.communityMemberships)
            {
                Label communityOption = new Label(community.community.name);
                communityOption.style.unityFontStyleAndWeight = FontStyle.Bold;

                communityOption.RegisterCallback<ClickEvent>(ChangeCommunity);
                communityOptions.Add(communityOption);
                communityNames.Add(community.community.name);
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
