
using System;
using PJKT.SDK.Prefabs;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace PJKT.SDK.Prefabs
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [RequireComponent(typeof(BoxCollider))]
    public class ResetPickups : UdonSharpBehaviour
    {
        [SerializeField] private VRCObjectSync[] PickupsToReset;
        [SerializeField] public bool isWhiteListed = false;
        [SerializeField] private string[] whiteList;

        private string localPlayerName;
        private VRC_Pickup[] _VRCRpickupsCache;
        private Collider _buttonCollider;
        private bool _hasAccess = false;
        
        private void Start()
        {
            localPlayerName = Networking.LocalPlayer.displayName;
            _buttonCollider = GetComponent<Collider>();
            
            // Check if the player is on the whitelist
            if (isWhiteListed)
            {
                foreach (string name in whiteList)
                {
                    if (localPlayerName == name)
                    {
                        _hasAccess = true;
                    }
                    // Else disable the button
                    else
                    {
                        _buttonCollider.enabled = false;
                    }
                }
            }
            // If the whitelist is disabled, give access to everyone
            else
            {
                _hasAccess = true;
            }
            
            // Find all VRC_Pickups on the objects
            _VRCRpickupsCache = new VRC_Pickup[PickupsToReset.Length];
            for (int i = 0; i < PickupsToReset.Length; i++)
            {
                _VRCRpickupsCache[i] = PickupsToReset[i].GetComponent<VRC_Pickup>();
            }
            
        }

        public override void Interact()
        {
            // Check if the player has access
            if (_hasAccess)
            {
                // Get the ownership of the object
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                
                // Drop all objects in _VRCpickupsCache and reset all pickups in PickupsToReset
                for (int i = 0; i < _VRCRpickupsCache.Length; i++)
                {
                    // Drop the object
                    _VRCRpickupsCache[i].Drop();
                    
                    // Reset the pickup
                    PickupsToReset[i].Respawn();
                }
            }
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(ResetPickups))]
    class ResetPickupsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ResetPickups _target = (ResetPickups)target;

            // Centered title in a new theme
            GUIStyle centeredStyleTitle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };
            centeredStyleTitle.fontStyle = FontStyle.Bold;
            centeredStyleTitle.fontSize = 16;

            EditorGUILayout.LabelField("PJKT Booth Reset Pickups", centeredStyleTitle);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("This script is used to reset pickups in the scene. It can be used to reset pickups for all player.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.HelpBox("Only available for pickups with VRCObjectSync", MessageType.Info);
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("PickupsToReset"), true);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isWhiteListed"));
            if (_target.isWhiteListed)
            {
                EditorGUILayout.HelpBox("Here you can specify the names of people who are the only ones allowed to use this button.", MessageType.Info);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("whiteList"), true);
                
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
