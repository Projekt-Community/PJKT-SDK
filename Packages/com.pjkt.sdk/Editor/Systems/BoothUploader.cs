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
        //private static string defaultPrefabPath = "Assets/PJKTCustomBooth/";
        //private static string defaultPackagePath = "Assets/PJKTCustomBooth/";
        
        //private static string prefabPath = "";
        private static string packagePath = "";
        
        public static async Task UploadBoothAsync(BoothDescriptor boothDescriptor)
        {
            bool success = false;
            if (boothDescriptor == null)
            {
                PjktSdkWindow.Notify("Tried to upload a booth that doesn't exist. How did you do that?", BoothErrorType.Warning);
                return;
            }
            
            PjktSdkWindow.Notify("Building...");

            long buildsize = PjktBuildSize.AssessBuildSize(boothDescriptor);
            if (buildsize == -1)
            {
                PjktSdkWindow.Notify("Failed to build booth. Ask for help on the discord.");
                return;
            }

            long maxBuildSizeBytes = PjktEventManager.SelectedProjekt.booth_requirements.MaxBuildSize * 1024 * 1024;
            if (buildsize > maxBuildSizeBytes)
            {
                PjktSdkWindow.Notify($"Booth exceeds maximum build size ({BoothValidator.FormatSize(buildsize)}/{PjktEventManager.SelectedProjekt.booth_requirements.MaxBuildSize}MB)\nTry compressing your textures or meshes.\n Booth was not uploaded.", BoothErrorType.Error);
                return;
            }

            PjktSdkWindow.Notify("Uploading booth to server...");

            PjktFileExporter exporter = new PjktFileExporter(boothDescriptor.currentCommunity);
            packagePath = exporter.CreateBoothfile(boothDescriptor.gameObject);

            if (string.IsNullOrEmpty(packagePath))
            {
                //failed to create zip for some reason
                PjktSdkWindow.Notify($"Failed to create booth package. Ask for help on the discord.", BoothErrorType.Error);
                return;
            }
            
            if (boothDescriptor.currentCommunity == "Debug")
            {
                PjktSdkWindow.Notify("Created Debug Booth. No upload necessary.");
                //AssetDatabase.Refresh();
                return;
            }
                
            success = await UploadBoothToServer(boothDescriptor);
            
            CleanupOperations(boothDescriptor);
            if (success) PjktSdkWindow.Notify("Booth Uploaded Successfully. See you at the event!");
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
                form.Add(new ByteArrayContent(fileBytes), "booth", booth.boothName + ".zip");
                form.Add(new ByteArrayContent(previewImageBytes), "preview", booth.boothName + " preview.png");
                
                
                //add the pjkt cookie
                CookieContainer container = new CookieContainer();
                container.Add(new Uri(PJKTNet.defaultHost), new Cookie("session", Authentication.PjktCookie));
                HttpClientHandler handler = new HttpClientHandler() { CookieContainer = container };
                HttpClient client = new HttpClient(handler);

                //byte[] bytes = await form.ReadAsByteArrayAsync();
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
            //really just need to remove the zip now
            if (string.IsNullOrEmpty(packagePath)) return;
            if (!File.Exists(packagePath)) return;
            if (Path.GetExtension(packagePath) != ".zip") return; //dont delete random stuff pls
            File.Delete(packagePath);
        }
    }
}
