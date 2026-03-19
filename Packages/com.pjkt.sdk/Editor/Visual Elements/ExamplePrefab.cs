using System;
using System.ComponentModel;
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
        private string _resourceURL;
        private string _packagePath;
        private string _prefabPath;

        public ExamplePrefab(string prefabName, string prefabDescription, string defaultAssetPath, string resourceURL)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            
            Name.text = prefabName;
            description.text = prefabDescription;
            
            _resourceURL = resourceURL;
            _packagePath = Path.GetTempPath() + "PjktSdk\\" + Name.text + ".unitypackage";
            _prefabPath = defaultAssetPath;

            if (File.Exists(defaultAssetPath)) button.text = "Add to Scene";
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
                //downloaded but not in this project
                if (File.Exists(_packagePath))
                {
                    AssetDatabase.ImportPackage(_packagePath, true);
                    button.text = "Add to Scene";
                    return;
                }
                
                //download path doesnt exist
                if (!Directory.Exists(Path.GetTempPath() + "PjktSdk\\")) Directory.CreateDirectory(Path.GetTempPath() + "PjktSdk\\");
                
                //download the file
                using (var webClient = new System.Net.WebClient())
                {
                    Uri uri = new Uri(_resourceURL);
                    webClient.DownloadFileCompleted += InstallPackage;
                    webClient.DownloadFileAsync(uri, _packagePath);
                }
            }
        }

        //called by the downloader
        private void InstallPackage(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            //check if it was sucessfull
            if (asyncCompletedEventArgs.Error != null)
            {
                Debug.LogError("Failed to download package: " + asyncCompletedEventArgs.Error);
                return;
            }
            
            //install the unity package
            AssetDatabase.ImportPackage(_packagePath, true);
            button.text = "Add to Scene";
        }
    }
}