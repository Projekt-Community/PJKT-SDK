
using UdonSharp;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRC.Udon;

namespace PJKT.SDK.Prefabs
{
    enum onPlayerLoadedAnimation
    {
        DoNothing,
        EnableAnimationValues,
        DisableAnimationValues
    }
    
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(BoxCollider))]
    public class AnimationToggle : UdonSharpBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string[] animationBooleansToToggle;
        [SerializeField] private onPlayerLoadedAnimation whenPlayerLoadsIn;
        [SerializeField] private bool canOnlyBeToggledOnce = false;
        
        private bool hasBeenToggledOnce = false;
        private bool isToggled = false;
        private Collider _buttonCollider;

        void Start()
        {
            _buttonCollider = GetComponent<Collider>();
            if (whenPlayerLoadsIn == onPlayerLoadedAnimation.EnableAnimationValues)
            {
                foreach (string value in animationBooleansToToggle)
                {
                    Debug.Log("Enabling " + value);
                    animator.SetBool(value, true);
                    isToggled = true;
                }
            }
            else if (whenPlayerLoadsIn == onPlayerLoadedAnimation.DisableAnimationValues)
            {
                foreach (string value in animationBooleansToToggle)
                {
                    Debug.Log("Disabling " + value);
                    animator.SetBool(value, false);
                    isToggled = false;
                }
            }
        }
        
        public override void Interact()
        {
            if (canOnlyBeToggledOnce)
            {
                _buttonCollider.enabled = false;
            }
            
            foreach (string value in animationBooleansToToggle)
            {
                animator.SetBool(value, !animator.GetBool(value));
            }
            
            isToggled = !isToggled;
            
            if (canOnlyBeToggledOnce)
            {
                hasBeenToggledOnce = true;
            }
        }
        
    }
    
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(AnimationToggle))]
    class AnimationToggleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AnimationToggle _target = (AnimationToggle)target;
                
            // Centered title in a new theme
            GUIStyle centeredStyleTitle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter};
            centeredStyleTitle.fontStyle = FontStyle.Bold;
            centeredStyleTitle.fontSize = 16;
            
            EditorGUILayout.LabelField("PJKT Booth Animation Toggle Script", centeredStyleTitle);
            EditorGUILayout.Space();
                
            EditorGUILayout.LabelField("Animator", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animator"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("animationBooleansToToggle"), true);
                
            EditorGUILayout.Space();
                
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("whenPlayerLoadsIn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canOnlyBeToggledOnce"));
                
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
