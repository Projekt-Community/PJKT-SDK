using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class ExamplePrefab : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ExamplePrefab> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/ExamplePrefab.uxml";
        public ExamplePrefab() { }
        
        private Button button => this.Q<Button>("InstallButton");
        private Label description => this.Q<Label>("Description");
        private Label Name => this.Q<Label>("Name");
        
        //used for checking if the prefab is installed or not
        private string _packagePath;
        private string _prefabPath;

        public ExamplePrefab(string prefabName, string prefabDescription, string prefabPath, string packagePath)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            Name.text = prefabName;
            description.text = prefabDescription;
            
            _packagePath = packagePath;
            _prefabPath = prefabPath;

            if (File.Exists(prefabPath)) button.text = "Add to Scene";
            else button.text = "Install";
            
            button.RegisterCallback<ClickEvent>(AddToScene);
        }
        
        private void AddToScene(ClickEvent evt)
        {
            if (File.Exists(_prefabPath))
            {
                //create it
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPath);
                GameObject obj = GameObject.Instantiate(prefab);
                
                //select it
                EditorGUIUtility.PingObject(obj);
                Selection.activeGameObject = obj;
            }
            else
            {
                AssetDatabase.ImportPackage(_packagePath, true);
                button.text = "Add to Scene";
            }
        }
    }
}