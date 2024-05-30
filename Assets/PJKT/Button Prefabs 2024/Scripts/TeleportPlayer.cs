using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace PJKT.SDK.Prefabs
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TeleportPlayer : UdonSharpBehaviour
    {
        public Transform teleportPlayerTo;


        public bool canOnlyBeUsedOnce = false;
        private bool hasBeenUsed = false;

        public override void Interact()
        {
            Networking.LocalPlayer.TeleportTo(teleportPlayerTo.position, teleportPlayerTo.rotation);
        }


    }
    
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(TeleportPlayer))]
    class TeleportPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Centered title in a new theme
            GUIStyle centeredStyleTitle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter};
            centeredStyleTitle.fontStyle = FontStyle.Bold;
            centeredStyleTitle.fontSize = 16;
            
            EditorGUILayout.LabelField("PJKT Booth Teleport", centeredStyleTitle);
            EditorGUILayout.LabelField("Use this script to teleport the player to a specific location", EditorStyles.helpBox);
            EditorGUILayout.Space();
                
            EditorGUILayout.LabelField("teleportPlayerTo", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("teleportPlayerTo"));
                
            EditorGUILayout.Space();
                
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canOnlyBeUsedOnce"));
                
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
