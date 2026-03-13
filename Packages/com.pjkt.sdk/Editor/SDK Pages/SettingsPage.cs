using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class SettingsPage : SDKPage
    {
        private const string sdkPrefabsListUrl = "https://raw.githubusercontent.com/ProjektCommunity/PJKT-SDK-Prefabs/refs/heads/main/PrefabsList.json";
        private PjktPrefabsList prefabsList;
        
        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;
            
            SettingsPannel panel = new SettingsPannel();
            panel.style.flexGrow = 1;
            topArea.Add(panel);
            
            //warning for removing old version of packages
            BoothError warning = new  BoothError("If you have previously installed any of the example packages, please remove them before installing the new versions. Everything should be under Assets/PJKT.", BoothErrorType.Warning);
            topArea.Add(warning);
            
            //get the json and parse
            FetchPrefabsJson();
        }

        private async void FetchPrefabsJson()
        {
            string response;
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                try
                {
                    response = await httpClient.GetStringAsync(sdkPrefabsListUrl);
                    prefabsList = JsonUtility.FromJson<PjktPrefabsList>(response);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Failed to fetch prefabs list: " + e.Message);
                }
                
                if (prefabsList == null) return;
                CreateButtons();
            }
        }

        private void CreateButtons()
        {
            foreach (PjktPrefabsInfo prefab in prefabsList.Prefabs)
            {
                ExamplePrefab prefabButton = new ExamplePrefab(prefab.PrefabName, prefab.PrefabDescription, prefab.DefaultAssetPath, prefab.ResourceURL);
                scrollView.Add(prefabButton);
            }
        }
    }
}