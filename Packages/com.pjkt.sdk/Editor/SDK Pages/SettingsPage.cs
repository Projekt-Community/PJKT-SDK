using System.IO;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SettingsPage : SDKPage
    {
        private const string unityPackagesBasePath = "Packages/com.pjkt.sdk/Runtime/Examples/";
        
        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;
            
            SettingsPannel panel = new SettingsPannel();
            panel.style.flexGrow = 1;
            topArea.Add(panel);
            
            //warning for removing old version of packages
            BoothError warning = new  BoothError("If you have previously installed any of the example packages, please remove them before installing the new versions. Everything should be under Assets/PJKT.", BoothErrorType.Warning);
            topArea.Add(warning);
            
            //grab all unitypackages in the examples folder and add them to the scroll view. would be nice but need more info that cant be provided
            /*string[] packagePaths = Directory.GetFiles(unityPackagesBasePath, "*.unitypackage", SearchOption.AllDirectories);
            foreach (string packagePath in packagePaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(packagePath);
                string displayName = fileName.Replace("_", " ");
                ExamplePrefab prefab = new ExamplePrefab(displayName, "An example prefab from the SDK examples folder", "", packagePath);
                scrollView.Add(prefab);
            }*/
            
            //booth prefabs
            ExamplePrefab prefabs = new ExamplePrefab("PJKT Booth Prefabs", "A collection of prefabs for creating interactions in your booth", "Assets/PJKT/Pjkt Prefabs/Open Group UI.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Pjkt Prefabs.unitypackage");
            scrollView.Add(prefabs);
            
            //2023 prefab booth
            ExamplePrefab booth2023 = new ExamplePrefab("Fest2023 Prefab Booth", "A simple booth used in the 2023 Projekt:Fest event", "Assets/PJKT/Booth Fest 2023/Booth Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Fest23 Prefab Booth.unitypackage");
            scrollView.Add(booth2023);
            
            //2024 booth prefab
            ExamplePrefab booth2024 = new ExamplePrefab("Fest2024 Prefab Booth", "A simple booth used in the 2024 Projekt:Fest event", "Assets/PJKT/Booth Fest 2024/Booth Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Fest24 Prefab Booth.unitypackage");
            scrollView.Add(booth2024);
            
            //2025 booth prefab
            ExamplePrefab booth2025 = new ExamplePrefab("Fest2025 Prefab Booth", "A simple booth used in the 2025 Projekt:Fest event", "Assets/PJKT/Booth Fest 2025/PJKT Booth 2025 Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Fest25 Prefab Booth.unitypackage");
            scrollView.Add(booth2025);
            
            //fang 2026 booth
            ExamplePrefab fang2026 = new ExamplePrefab("Fang2026 Prefab Booth", "A simple booth used in the 2026 Projekt:Fang event", "Assets/PJKT/Booth Fang 2026/Prefab Booth.fbx", "Packages/com.pjkt.sdk/Runtime/Examples/Fang26 Prefab Booth.unitypackage");
            scrollView.Add(fang2026);
            
            //horrorcon 2024 booth
            ExamplePrefab horrorcon2024 = new ExamplePrefab("Horrorcon2024 Prefab Booth", "A simple booth used in the 2024 Projekt:Horrorcon event", "Assets/PJKT/Booth Horrorcon 2024/HorrorCon 24 Prefab Booth.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Horrorcon24 Prefab Booth.unitypackage");
            scrollView.Add(horrorcon2024);
            
            //horrorcon 2025 booth
            ExamplePrefab horrorcon2025 = new ExamplePrefab("Horrorcon2025 Prefab Booth", "A simple booth used in the 2025 Projekt:Horrorcon event", "Assets/PJKT/Booth Horrorcon 2025/HorrorCon 25 Prefab Booth.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/Horrorcon25 Prefab Booth.unitypackage");
            scrollView.Add(horrorcon2025);
        }
    }
}