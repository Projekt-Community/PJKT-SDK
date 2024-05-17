using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class TexturesPage : SDKPage
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
            
            //get report
            BoothStats textures = BoothValidator.Report.GetStats(StatsType.Textures);
            
            //create top info area
            infoBox = new RequirementsInfo(new BoothStats[] {textures});
            topArea.Add(infoBox);
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //create info pannels
            foreach (TextureInfo texture in textures.ComponentList)
            {
                string textureInfo = $"Base Resolution: {texture.pixelSize[0]} x {texture.pixelSize[1]} \n"
                                     + $"Max Texture Size: {texture.importedSize} \n"
                                     + $"Format: {texture.filetype} \n"
                                     + $"Vram: {BoothValidator.FormatSize(texture.vRamSize)}\n\n";
                
                string matsinfo = "Used in Materials: \n";
                foreach (Material mat in texture.materials) matsinfo += mat.name + "\n";
                
                InfoPanel panel = new InfoPanel(texture.texture, AssetPreview.GetMiniTypeThumbnail(typeof(Texture)), "Texture", textureInfo + matsinfo, PjktGraphics.GraphicColors["Texture"]);
                
                scrollView.Add(panel);   
            }
        }
    }
}