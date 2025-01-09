using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace PJKT.SDK2
{
    /// <summary>
    /// Builds an asset bundle of the current booth to check the build size.
    /// idk if thats how we wanna do it but it appears to be what vitdeck does
    /// </summary>
    public static class PjktBuildSize
    {
#if UNITY_EDITOR
        //temp for testing
        [MenuItem("PJKT/TestBuildSize")]
        public static void Test()
        {
            //get descriptor from selected object
            GameObject obj = Selection.activeObject as GameObject;
            BoothDescriptor booth = obj.GetComponent<BoothDescriptor>();
            
            if (booth == null)
            {
                Debug.LogError("Selected object does not have a booth descriptor");
                return;
            }
            
            AssessBuildSize(booth);
        }
        
        //plan is to create a new temporary scene, copy the booth to it and build that scene into an asset bundle
        public static long AssessBuildSize(BoothDescriptor booth)
        {
            //prolly need to do some sort of editor lock or progress bar here
            
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            string tempFilePath = Path.GetTempPath() + "PjktSdk\\";
            string scenePath = "Assets/PjktTemp/" + booth.currentCommunity + "_BuildSizeTemp.unity";
            
            AssetBundleManifest manifest = null;
            
            try
            {
                GameObject boothCopy = GameObject.Instantiate(booth.gameObject);
                SceneManager.MoveGameObjectToScene(boothCopy, scene);

                if (!Directory.Exists("Assets/PjktTemp/")) Directory.CreateDirectory("Assets/PjktTemp/");

                EditorSceneManager.SaveScene(scene, scenePath);

                AssetBundleBuild build = new AssetBundleBuild
                {
                    assetBundleName = $"{booth.currentCommunity}_BuildSizeTemp",
                    assetNames = new[] { scenePath }
                };

                manifest = BuildPipeline.BuildAssetBundles(tempFilePath, new[] { build }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //figure out android later
            }
            catch (Exception e)
            {
                throw e;
                return 0;
            }
            finally
            {
                //get rid of temp scene
                EditorSceneManager.CloseScene(scene, true);
            }

            if (manifest == null) return 0;
            if (!File.Exists(tempFilePath + $"{booth.currentCommunity}_BuildSizeTemp")) return 0;
                
            FileInfo fileInfo = new FileInfo(tempFilePath + $"{booth.currentCommunity}_BuildSizeTemp");
            
            Debug.Log("Build size: " + fileInfo.Length + " bytes");
            
            return fileInfo.Length;
        }
#endif
    }
}