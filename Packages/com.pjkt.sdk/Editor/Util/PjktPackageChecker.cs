using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace PJKT.SDK2
{
    public static class PjktPackageChecker
    {
        internal static Dictionary<string, string> PackageSources { get; private set; } = new Dictionary<string, string>();

        [MenuItem("PJKT SDK/Tools/Refresh Package List")]
        public static async void ListAllPackages()
        {
            ListRequest req = Client.List();
            while (!req.IsCompleted) await Task.Delay(100);
            {
                if (req.Status == StatusCode.Success)
                {
                    foreach (var package in req.Result)
                    {
                        //store the package name and version
                        PackageSources[package.name] = package.version;
                    }
                }
                else PjktSdkWindow.Notify("Failed to find packages installed in this project, some warnings may not appear.");
            }
        }
    }
}