using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace PJKT.SDK2
{
     [CustomEditor(typeof(PjktMaterialSwapper))]
    public class PjktMaterialSwapperEditor : Editor
    {
        private Texture2D headerImage;
        private Texture2D backgroundImage;

        private Color windowsBackgroundColor = new Color(.3f, .5f, 1f, 1);
        private Color androidBackgroundColor = new Color(.3f, .9f, .5f, 1);
        private Color iosBackgroundColor = new Color(.8f, .5f, 1f, 1);
        
        private PjktMaterialSwapper swapper;
        
        private SerializedProperty windowsMats;
        private SerializedProperty androidMats;
        private SerializedProperty iosMats;
        
        void OnEnable()
        {
            headerImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/PJKT/Editor/Resources/header.png");
            backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/PJKT/Editor/Resources/background.png");
            
            swapper = target as PjktMaterialSwapper;
            windowsMats = serializedObject.FindProperty("windowsMaterials");
            androidMats = serializedObject.FindProperty("androidMaterials");
            iosMats = serializedObject.FindProperty("iosMaterials");
        }

        public override void OnInspectorGUI()
        {
            PjktCommonGui.DrawComponentHeader("Material Swapper", "Use this to assign different material sets per build target");
            
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            serializedObject.Update();
            
            GUI.backgroundColor = windowsBackgroundColor;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.backgroundColor = Color.white;
                EditorGUILayout.LabelField("Windows", style, GUILayout.ExpandWidth(true));
                GUILayout.Space(3);

                for (int i = 0; i < windowsMats.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(windowsMats.GetArrayElementAtIndex(i));
                }

                if (GUILayout.Button("Grab current materials"))
                {
                    windowsMats.ClearArray();
                    windowsMats.arraySize = swapper.rend.sharedMaterials.Length;
                    for (int i = 0; i < windowsMats.arraySize; i++)
                    {
                        windowsMats.GetArrayElementAtIndex(i).objectReferenceValue = swapper.rend.sharedMaterials[i];
                    }
                }
            }
            GUILayout.Space(5);
            
            GUI.backgroundColor = androidBackgroundColor;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.backgroundColor = Color.white;
                EditorGUILayout.LabelField("Android", style, GUILayout.ExpandWidth(true));
                GUILayout.Space(3);
                
                for (int i = 0; i < androidMats.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(androidMats.GetArrayElementAtIndex(i));
                }
                
                if (GUILayout.Button("Grab current materials"))
                {
                    androidMats.ClearArray();
                    androidMats.arraySize = swapper.rend.sharedMaterials.Length;
                    for (int i = 0; i < androidMats.arraySize; i++)
                    {
                        androidMats.GetArrayElementAtIndex(i).objectReferenceValue = swapper.rend.sharedMaterials[i];
                    }
                }
            }
            GUILayout.Space(5);
            
            GUI.backgroundColor = iosBackgroundColor;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.backgroundColor = Color.white;
                EditorGUILayout.LabelField("IOS", style, GUILayout.ExpandWidth(true));
                GUILayout.Space(3);
                
                for (int i = 0; i < iosMats.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(iosMats.GetArrayElementAtIndex(i));
                }
                
                if (GUILayout.Button("Grab current materials"))
                {
                    iosMats.ClearArray();
                    iosMats.arraySize = swapper.rend.sharedMaterials.Length;
                    for (int i = 0; i < iosMats.arraySize; i++)
                    {
                        iosMats.GetArrayElementAtIndex(i).objectReferenceValue = swapper.rend.sharedMaterials[i];
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    public class PjktMaterialSwapOnBuildTargetSwitch : IActiveBuildTargetChanged
    {
        public int callbackOrder { get; }
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Debug.Log("target swapped from " + previousTarget + " to " + newTarget);
            //find and swap on all material swappers
            PjktMaterialSwapper[] swappers = Object.FindObjectsOfType<PjktMaterialSwapper>(true);
            int buildTargetIndex = -1;
            
            switch (newTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    buildTargetIndex = 0;
                    break;
                
                case BuildTarget.Android:
                    buildTargetIndex = 1;
                    break;
                
                case BuildTarget.iOS:
                    buildTargetIndex = 2;
                    break;
            }
            if (buildTargetIndex == -1) return;

            foreach (PjktMaterialSwapper materialSwapper in swappers)
            {
                if (materialSwapper == null) continue;
                materialSwapper.SwapMaterialsToTarget(buildTargetIndex);
                EditorUtility.SetDirty(materialSwapper);
            }
        }
    }
}
