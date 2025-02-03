using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDK3.Editor;

namespace PJKT.SDK2
{
    public class PjktTestScene
    {
        public readonly BoothDescriptor Booth;
        public readonly PjktTestSceneOptions Options;
        private string scenePath;
        private Scene previousScene;

        public PjktTestScene(BoothDescriptor booth, PjktTestSceneOptions options)
        {
            Booth = booth;
            Options = options;
        }

        public async Task<bool> BuildAndTestBooth()
        {
            previousScene = SceneManager.GetActiveScene(); //save the current scene were in
            EditorSceneManager.SaveScene(previousScene);
            
            //create and open the new test scene
            CreateTestScene();
            
            //close the previous scene
            EditorSceneManager.CloseScene(previousScene, false);
            
            //have vrchat sdk build and test scene
            if (!VRCSdkControlPanel.TryGetBuilder<IVRCSdkWorldBuilderApi>(out var builder))
            {
                PjktSdkWindow.Notify("Unable to communicate with VRChat's SDK. Try opening it to the builder page.", BoothErrorType.Error);
                CleanupTestfiles();
                return false;
            }
            
            try
            {
                await builder.BuildAndTest();
            }
            catch (Exception e)
            {
                PjktSdkWindow.Notify($"VRCSDK ran into an error, make sur you are logged in and the SDK is opedn to the Builder page.\nError: {e.Message}");
                CleanupTestfiles();
                return false;
            }
            
            //clean up temp scene and files
            CleanupTestfiles();

            return true;
        }
        private void CreateTestScene()
        {
            Scene testScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
            
            //add the booth, a floor collider, and the vrc world prefab
            GameObject boothclone = GameObject.Instantiate(Booth.gameObject);
            EditorSceneManager.MoveGameObjectToScene(boothclone, testScene);
            boothclone.transform.position = Vector3.zero;
            boothclone.transform.rotation = Quaternion.identity;

            GameObject floor = new GameObject();
            BoxCollider floorCollider = floor.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(100f, .01f, 100);
            EditorSceneManager.MoveGameObjectToScene(floor, testScene);
            floor.transform.position = Vector3.zero;
            floor.transform.rotation = Quaternion.identity;
            
            GameObject vrcWorldPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.vrchat.worlds/Samples/UdonExampleScene/Prefabs/VRCWorld.prefab");
            GameObject vrcWorld = GameObject.Instantiate(vrcWorldPrefab);
            EditorSceneManager.MoveGameObjectToScene(vrcWorld, testScene);
            vrcWorld.transform.position = new Vector3(0,0,10);
            vrcWorld.transform.rotation = Quaternion.Euler(0, 180, 0);
            
            //here is where we would make use of the scene options
            
            //save it
            if (!Directory.Exists("Assets/PjktTemp/")) Directory.CreateDirectory("Assets/PjktTemp/");
            scenePath = $"Assets/PjktTemp/{Booth.boothName}_TestScene.unity";
            EditorSceneManager.SaveScene(testScene, scenePath);
        }
        
        private void CleanupTestfiles()
        {
            //reopen old scene
            EditorSceneManager.OpenScene(previousScene.path, OpenSceneMode.Single);
            
            if (File.Exists(scenePath)) File.Delete(scenePath);
        }
    }

    public class PjktTestSceneOptions
    {
        //empty atm
    }
}