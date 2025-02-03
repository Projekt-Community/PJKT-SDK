using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
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
                Debug.LogError("<color=#FFBB00><b>PJKT SDK:</b></color> Selected object does not have a booth descriptor");
                return;
            }
            
            long size = AssessBuildSize(booth);

            if (size == -1)
            {
                Debug.Log("<color=#FFBB00><b>PJKT SDK:</b></color> Failed to build booth");
                return;
            }

            Debug.Log("<color=#FFBB00><b>PJKT SDK:</b></color> Build size: " + BoothValidator.FormatSize(size));
        }
        
        //plan is to create a new temporary scene, copy the booth to it and build that scene into an asset bundle
        public static long AssessBuildSize(BoothDescriptor booth)
        {
            //prolly need to do some sort of editor lock or progress bar here
            string tempFilePath = Path.GetTempPath() + "PjktSdk\\";
            string prefabPath = "Assets/PjktTemp/" + booth.currentCommunity + "_BuildSizeTemp.prefab";
            
            AssetBundleManifest manifest = null;
            
            try
            {
                if (!Directory.Exists("Assets/PjktTemp/")) Directory.CreateDirectory("Assets/PjktTemp/");
                if (!Directory.Exists(tempFilePath)) Directory.CreateDirectory(tempFilePath);
                PrefabUtility.SaveAsPrefabAsset(booth.gameObject, prefabPath);

                AssetBundleBuild build = new AssetBundleBuild
                {
                    assetBundleName = $"{booth.currentCommunity}_BuildSizeTemp",
                    assetNames = new[] { prefabPath }
                };

                manifest = BuildPipeline.BuildAssetBundles(tempFilePath, new[] { build }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64); //figure out android later
            }
            catch (Exception e)
            {
                throw e;
            }

            if (manifest == null) return -1;
            
            if (!File.Exists(tempFilePath + $"{booth.currentCommunity}_BuildSizeTemp")) return -1;
                
            FileInfo fileInfo = new FileInfo(tempFilePath + $"{booth.currentCommunity}_BuildSizeTemp");
            long builtSize = fileInfo.Length;
            
            //get rid of temp prefab
            if (File.Exists(prefabPath)) File.Delete(prefabPath);
            if (File.Exists(prefabPath + ".meta")) File.Delete(prefabPath + ".meta");
                
            AssetDatabase.Refresh();
            
            return builtSize;
        }
#endif
    }
}