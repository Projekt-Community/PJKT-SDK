using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SettingsPage : SDKPage
    {
        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;
            
            SettingsPannel panel = new SettingsPannel();
            panel.style.flexGrow = 1;
            topArea.Add(panel);
            
            //2023 prefab booth
            ExamplePrefab booth2023 = new ExamplePrefab("PJKT2023 Prefab Booth", "A simple booth used in the 2023 Projekt:Fest event", "Assets/PJKT/Booth 2023/Booth Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/PJKT2023PrefabBooth.unitypackage");
            scrollView.Add(booth2023);
            
            //2024 booth prefab
            ExamplePrefab booth2024 = new ExamplePrefab("PJKT2024 Prefab Booth", "A simple booth used in the 2024 Projekt:Fest event", "Assets/PJKT/Booth 2024/Booth Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/PJKT2024PrefabBooth.unitypackage");
            scrollView.Add(booth2024);
            
            //2024 booth prefab
            ExamplePrefab booth2025 = new ExamplePrefab("PJKT2025 Prefab Booth", "A simple booth used in the 2025 Projekt:Fest event", "Assets/PJKT/Prefab Booth 2025/PJKT Booth 2025 Prefab.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/PJKT Booth 2025.unitypackage");
            scrollView.Add(booth2025);
            
            //2025button prefabs
            ExamplePrefab button2025 = new ExamplePrefab("PJKT2025 Button Prefabs", "A collection of buttons for toggling objects and animations", "Assets/PJKT/Button Prefabs 2025/Button Prefabs/Object Toggles/PJKT Toggle Button.prefab", "Packages/com.pjkt.sdk/Runtime/Examples/PJKT Button Prefabs 2025.unitypackage");
            scrollView.Add(button2025);
        }
    }
}