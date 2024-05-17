using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class TextsPage : SDKPage
    {
        private RequirementsInfo infoBox;
        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;

            if (BoothValidator.BoothsInScene.Length == 0)
            {
                topArea.Add(new NoBoothsMessage());
                return;
            }
            
            //ports again
            BoothStats texts = BoothValidator.Report.GetStats(StatsType.TMProTexts);
            
            //top box 
            infoBox = new RequirementsInfo(new BoothStats[] {texts});
            topArea.Add(infoBox);
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //bottom box
            foreach (TMP_Text text in texts.ComponentList)
            {
                string textInfo =   $"Font: {text.font}\n\n" + $"Text: {text.text}";
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.textmeshpro/Editor Resources/Gizmos/TMP - Text Component Icon.psd");
                InfoPanel panel = new InfoPanel(text.gameObject, icon, "TextMeshPro", textInfo, PjktGraphics.GraphicColors["Text"]);
                scrollView.Add(panel);
            }
        }
    }
}