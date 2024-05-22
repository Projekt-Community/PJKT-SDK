using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PJKT.SDK2.NET;
using UnityEditor;
using UnityEngine;
using Directory = UnityEngine.Windows.Directory;

namespace PJKT.SDK2
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
                PjktSdkWindow.Notify("Tried to upload a booth that doesn't exist. How did you do that?", BoothErrorType.Warning);
                return;
            }

            PjktSdkWindow.Notify("Uploading booth to server...");
            
            if (await CreateBoothPackage(boothDescriptor))
            {
                success = await UploadBoothToServer(boothDescriptor);
            }
            
            CleanupOperations(boothDescriptor);
            if (success) PjktSdkWindow.Notify("Booth Uploaded Successfully. See you at the event!");
        }

        private static async Task<bool> CreateBoothPackage(BoothDescriptor boothDescriptor)
        {
            prefabPath = defaultPrefabPath + boothDescriptor.currentCommunity +"/" + boothDescriptor.boothName + ".prefab";
            packagePath = defaultPackagePath + boothDescriptor.currentCommunity +"/" + boothDescriptor.boothName + ".unitypackage";
            
            try
            {
                //Turn the booth into a prefab
                CreateDirectory(prefabPath);
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(boothDescriptor.gameObject, prefabPath);
                AssetDatabase.ImportAsset(prefabPath);
            
                //Wait for this file to exist
                while (!File.Exists(prefabPath)) { await Task.Delay(100); }

                //TODO: Strip components from the prefab, including Bakery components and unoptimized Poiyomi shaders
                //Really anything that will cause unnecessary bloat
                //Perhaps all but whitelisted components?

                //Export the UnityPackage
                CreateDirectory(packagePath);

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
                PjktSdkWindow.Notify($"Booth upload failed, Couldn't create booth package\n {e}", BoothErrorType.Error);
                return false;
            }
        }

        private static async Task<bool> UploadBoothToServer(BoothDescriptor booth)
        {
            try
            {
                //Upload the UnityPackage
                byte[] fileBytes = File.ReadAllBytes(packagePath);
                
                Texture2D previewImage = AssetPreview.GetAssetPreview(booth.gameObject);
                byte[] previewImageBytes = previewImage.EncodeToPNG();
                
                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent(PjktEventManager.SelectedProjekt.id.ToString()), "project_id");
                form.Add(new StringContent(Authentication.ActiveUser.GetCommunityId(booth.currentCommunity).ToString()), "community_id");
                form.Add(new ByteArrayContent(fileBytes), "booth", booth.boothName + ".unitypackage");
                form.Add(new ByteArrayContent(previewImageBytes), "preview", booth.boothName + " preview.png");
                
                
                //add the pjkt cookie
                CookieContainer container = new CookieContainer();
                container.Add(new Uri(PJKTNet.defaultHost), new Cookie("session", Authentication.PjktCookie));
                HttpClientHandler handler = new HttpClientHandler() { CookieContainer = container };
                HttpClient client = new HttpClient(handler);

                byte[] bytes = await form.ReadAsByteArrayAsync();
                HttpResponseMessage response = await client.PostAsync(PJKTNet.defaultHost + "/booth/submit", form);
                
                if (response.StatusCode == HttpStatusCode.OK) return true;
                else
                {
                    PjktSdkWindow.Notify($"Failed to upload booth to server :( \n {response}", BoothErrorType.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                PjktSdkWindow.Notify("Failed to upload booth to server. '\n'" + e, BoothErrorType.Error);
                return false;
            }
        }

        private static void CleanupOperations(BoothDescriptor booth)
        {
            //Delete the temporary files and folders
            if (File.Exists(prefabPath)) File.Delete(prefabPath);
            string prefabFolderPath = prefabPath.TrimEnd('.','p','r','e','f','a','b');
            if (File.Exists(prefabFolderPath + ".meta")) File.Delete(prefabFolderPath + ".meta");
            
            if (File.Exists(packagePath)) File.Delete(packagePath);
            string packagefolderPath = packagePath.TrimEnd('.','u','n','i','t','y','p', 'a', 'c', 'k', 'a', 'g', 'e');
            if (File.Exists(packagefolderPath + ".meta")) File.Delete(packagefolderPath + ".meta");
            
            if (UnityEngine.Windows.Directory.Exists(Path.GetDirectoryName(prefabFolderPath))) UnityEngine.Windows.Directory.Delete(Path.GetDirectoryName(prefabFolderPath));
            if (UnityEngine.Windows.Directory.Exists(Path.GetDirectoryName(prefabFolderPath)  + ".meta")) Directory.Delete(Path.GetDirectoryName(prefabFolderPath) + ".meta");
            if (UnityEngine.Windows.File.Exists(defaultPrefabPath + booth.currentCommunity +".meta")) File.Delete(defaultPrefabPath + booth.currentCommunity +".meta");

            prefabPath = "";
            packagePath = "";
            
            //Refresh the asset database
            AssetDatabase.Refresh();
        }
        
        public static void CreatePath(string file)
        {
            if (Directory.Exists(file)) return;
            CreatePath(Path.GetDirectoryName(file));
            Directory.CreateDirectory(file);
        }

        public static void CreateDirectory(string file)
        {
            CreatePath(Path.GetDirectoryName(file));
        }
    }
}
