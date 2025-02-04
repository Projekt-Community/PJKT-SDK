#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

using UnityEngine;
using VRC.SDKBase;

namespace PJKT.SDK2
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    [AddComponentMenu("PJKT/Material Swapper")]
    public class PjktMaterialSwapper : MonoBehaviour
    {
        public Renderer rend;
        [SerializeField] private Material[] windowsMaterials;
        [SerializeField] private Material[] androidMaterials;
        [SerializeField] private Material[] iosMaterials;
        
        //0 = windows, 1 = android, 2 = IOS
        public void SwapMaterialsToTarget(int target)
        {
            //apply the target materials
            switch (target)
            {
                case 0:
                    rend.sharedMaterials = windowsMaterials;
                    break;
                case 1:
                    rend.sharedMaterials = androidMaterials;
                    break;
                case 2:
                    rend.sharedMaterials = iosMaterials;
                    break;
            }
        }

        private void Reset()
        {
            //called when component is added. used to get the renderer and materials
            rend = GetComponent<Renderer>();
            Material[] rendererMats = rend.sharedMaterials;
            
            //set all arrays to this for now
            windowsMaterials = rendererMats;
            androidMaterials = rendererMats;
            iosMaterials = rendererMats;
        }
    }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
    [CustomEditor(typeof(PjktMaterialSwapper))]
    public class PjktMaterialSwapperEditor : Editor, IActiveBuildTargetChanged
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
        public int callbackOrder { get; }
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            PjktMaterialSwapper swapper = target as PjktMaterialSwapper;
            switch (newTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    swapper.SwapMaterialsToTarget(0);
                    break;
                
                case BuildTarget.Android:
                    swapper.SwapMaterialsToTarget(1);
                    break;
                
                case BuildTarget.iOS:
                    swapper.SwapMaterialsToTarget(2);
                    break;
            }
        }
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
            serializedObject.Update();
            //thanks pesky
            if (headerImage != null && backgroundImage != null)
            {
                Rect headerRect = EditorGUILayout.GetControlRect(false, 80, GUILayout.ExpandWidth(true));
                
                GUI.DrawTexture(headerRect, backgroundImage, ScaleMode.ScaleAndCrop);
                
                float leftMargin = 10f;
                float logoSize = headerRect.height;
                Rect logoRect = new Rect(headerRect.x + leftMargin, headerRect.y, logoSize, logoSize);
                
                GUI.DrawTexture(logoRect, headerImage, ScaleMode.ScaleToFit);
                
                GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                
                GUIStyle subtitleStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 14,
                    normal = { textColor = Color.white }
                };
                
                float textLeftMargin = logoSize + leftMargin + 10;
                Rect titleRect = new Rect(headerRect.x + textLeftMargin, headerRect.y + 15, 
                    headerRect.width - textLeftMargin, headerRect.height / 2 - 5);
                Rect subtitleRect = new Rect(headerRect.x + textLeftMargin, headerRect.y + headerRect.height / 2 - 5, 
                    headerRect.width - textLeftMargin, headerRect.height / 2);
                
                GUI.Label(titleRect, "Material Swapper", titleStyle);
                GUI.Label(subtitleRect, "Use this to assign different material sets per build target", subtitleStyle);
                
                EditorGUILayout.Space(5);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                var leftAlignedStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                };
                EditorGUILayout.LabelField("Material Swapper", leftAlignedStyle);
                EditorGUILayout.LabelField("Use this to assign different material sets per build target", new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft, wordWrap = true, fontSize = 14 });
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }
            
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            
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
#endif 
}