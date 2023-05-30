using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using PJKT.SDK.Window;
using PJKT.SDK.NET;
using PJKT.SDK.NET.Messages;
using UnityEngine;
using UnityEditor;
using Directory = UnityEngine.Windows.Directory;


namespace PJKT.SDK
{
    internal static class BoothUploader
    {
        private static string defaultPrefabPath = "Assets/PJKTCustomBooth/";
        private static string defaultPackagePath = "Assets/PJKTCustomBooth/";
        
        private static string prefabPath = "";
        private static string packagePath = "";
        
        public static async Task UploadBoothAsync(BoothDescriptor boothDescriptor)
        {
            bool success = false;
            if (boothDescriptor == null)
            {
                Debug.LogWarning("<color=#4557f7>PJKT SDK</color>: No booth selected");
                return;
            }
            Debug.Log("<color=#4557f7>PJKT SDK</color>: starting booth upload");
            if (await CreateBoothPackage(boothDescriptor))
            {
                success = await UploadBoothToServer();
            }
            
            CleanupOperations();
            if (success) EditorUtility.DisplayDialog("Booth Uploaded Successfully", "Your booth was uploaded to the server. :)" , "Yay!");
        }

        private static async Task<bool> CreateBoothPackage(BoothDescriptor boothDescriptor)
        {
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Packaging up booth");
            prefabPath = defaultPrefabPath + AuthData.communityName +"/" + boothDescriptor.boothName + ".prefab";
            packagePath = defaultPackagePath + AuthData.communityName +"/" + boothDescriptor.boothName + ".unitypackage";
            
            try
            {
                //Turn the booth into a prefab
                PJKTUtil.CreateDirectory(prefabPath);
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boothDescriptor.gameObject, prefabPath);
                AssetDatabase.ImportAsset(prefabPath);
            
                //Wait for this file to exist
                while (!File.Exists(prefabPath)) { await Task.Delay(100); }

                //TODO: Strip components from the prefab, including Bakery components and unoptimized Poiyomi shaders
                //Really anything that will cause unnecessary bloat
                //Perhaps all but whitelisted components?

                //Export the UnityPackage
                PJKTUtil.CreateDirectory(packagePath);

                //Assemble the export options
                ExportPackageOptions packageOptions = ExportPackageOptions.Default;
                packageOptions |= ExportPackageOptions.Recurse;
                packageOptions |= ExportPackageOptions.IncludeDependencies;

                AssetDatabase.ExportPackage(new string[] { prefabPath }, packagePath, packageOptions);

                //Wait for this file to exist
                while (!File.Exists(packagePath)) { await Task.Delay(100); }

                return true;
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Booth Upload Error", "Couldn't create booth package :(", "Oh ok");
                Debug.LogError("<color=#4557f7>PJKT SDK</color>: Error creating booth package. " + e);
                return false;
            }
        }

        private static async Task<bool> UploadBoothToServer()
        {
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Uploading to PJKT");
            try
            {
                //Upload the UnityPackage
                byte[] fileBytes = File.ReadAllBytes(packagePath);
                //TODO: Photo

                PJKTUploadMessage uploadMessage = new PJKTUploadMessage(fileBytes, new Texture2D(4, 4).EncodeToPNG());
                
                HttpResponseMessage response;
                response = await PJKTNet.SendMessage(uploadMessage);
                
                //check if there is an issue with response
                string responseJson = await response.Content.ReadAsStringAsync();
                //Debug.Log(responseJson);
                PjktResponseObject responseObject = JsonUtility.FromJson<PjktResponseObject>(responseJson);
                if (responseObject.error)
                {
                    EditorUtility.DisplayDialog("Booth Upload Error", "Couldn't upload booth to server :(", "Oh ok");
                    Debug.LogError("<color=#4557f7>PJKT SDK</color>: Error uploading booth. " + responseObject.message);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Booth Upload Error", "Couldn't upload booth to server :(", "Oh ok");
                Debug.LogError("<color=#4557f7>PJKT SDK</color>: Error uploading booth. " + e);
                return false;
            }
        }

        private static void CleanupOperations()
        {
            //Delete the temporary files and folders
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Cleaning up temporary files");
            if (File.Exists(prefabPath)) File.Delete(prefabPath);
            string prefabFolderPath = prefabPath.TrimEnd('.','p','r','e','f','a','b');
            if (File.Exists(prefabFolderPath + ".meta")) File.Delete(prefabFolderPath + ".meta");
            
            if (File.Exists(packagePath)) File.Delete(packagePath);
            string packagefolderPath = packagePath.TrimEnd('.','u','n','i','t','y','p', 'a', 'c', 'k', 'a', 'g', 'e');
            if (File.Exists(packagefolderPath + ".meta")) File.Delete(packagefolderPath + ".meta");
            
            if (Directory.Exists(Path.GetDirectoryName(prefabFolderPath))) Directory.Delete(Path.GetDirectoryName(prefabFolderPath));
            if (Directory.Exists(Path.GetDirectoryName(prefabFolderPath)  + ".meta")) Directory.Delete(Path.GetDirectoryName(prefabFolderPath) + ".meta");
            if (UnityEngine.Windows.File.Exists(defaultPrefabPath + AuthData.communityName +".meta")) File.Delete(defaultPrefabPath + AuthData.communityName +".meta");

            prefabPath = "";
            packagePath = "";
            
            //Refresh the asset database
            Debug.Log("<color=#4557f7>PJKT SDK</color>: Refreshing AssetDatabase");
            AssetDatabase.Refresh();
        }
    }
}
