using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRC.Udon;


namespace PJKT.SDK.Prefabs
{
    public enum onPlayerLoaded
    {
        DoNothing,
        EnableAllObjects,
        DisableAllObjects
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(BoxCollider))]
    public class ButtonToggle : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] objectsAndComponentsToToggle;
        [SerializeField] private onPlayerLoaded whenPlayerLoadsIn;
        [SerializeField] private bool canOnlyBeToggledOnce = false;

        private bool hasBeenToggledOnce = false;
        private bool isToggled = false;
        private Collider _buttonCollider;

        void Start()
        {
            // Cache the collider
            _buttonCollider = GetComponent<Collider>();
            
            // Enable or disable the objects based on the setting
            if (whenPlayerLoadsIn == onPlayerLoaded.EnableAllObjects)
            {
                foreach (GameObject obj in objectsAndComponentsToToggle)
                {
                    obj.SetActive(true);
                    isToggled = true;
                }
            }
            else if (whenPlayerLoadsIn == onPlayerLoaded.DisableAllObjects)
            {
                foreach (GameObject obj in objectsAndComponentsToToggle)
                {
                    obj.SetActive(false);
                    isToggled = false;
                }
            }
        }

        public override void Interact()
        {
            // If the button can only be toggled once, disable the collider after the first press
            if (canOnlyBeToggledOnce)
            {
                _buttonCollider.enabled = false;
            }

            // Toggle the objects
            foreach (GameObject obj in objectsAndComponentsToToggle)
            {
                obj.SetActive(!isToggled);
            }

            // Toggle the state
            isToggled = !isToggled;
            hasBeenToggledOnce = true;
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(ButtonToggle))]
    class SeatTogglingManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ButtonToggle _target = (ButtonToggle)target;

            // Centered title in a new theme
            GUIStyle centeredStyleTitle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };
            centeredStyleTitle.fontStyle = FontStyle.Bold;
            centeredStyleTitle.fontSize = 16;

            EditorGUILayout.LabelField("PJKT Booth Toggle Script", centeredStyleTitle);
            EditorGUILayout.Space();
            

            EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsAndComponentsToToggle"), true);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("whenPlayerLoadsIn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canOnlyBeToggledOnce"));

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}