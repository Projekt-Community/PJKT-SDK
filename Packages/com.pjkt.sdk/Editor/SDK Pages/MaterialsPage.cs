using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class MaterialsPage : SDKPage
    {
        private RequirementsInfo requirementsInfo;

        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;

            if (BoothValidator.BoothsInScene.Length == 0)
            {
                topArea.Add(new NoBoothsMessage());
                return;
            }
            
            //get the reports
            BoothStats materials = BoothValidator.Report.GetStats(StatsType.Materials);
            
            //create the top info area
            requirementsInfo = new RequirementsInfo(new BoothStats[] {materials});
            topArea.Add(requirementsInfo);
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //check for custom shaders here and tell them to update
            //warnings
            List<BoothError> warnings = FindShadersToUpdate();
            if (warnings.Count > 0)
            {
                foreach (BoothError warning in warnings) topArea.Add(warning);
            }
            
            //fill out info box
            foreach (Material mat in materials.ComponentList)
            {
                List<string> texturePaths = new List<string>();
                foreach (string property in mat.GetTexturePropertyNames())
                {
                    if (mat.GetTexture(property) == null) continue;
                    string path = AssetDatabase.GetAssetPath(mat.GetTexture(property));
                    if (!texturePaths.Contains(path)) texturePaths.Add(path);
                }

                
                string info = "Shader: " + mat.shader.name + "\n"
                              + "Textures: " + texturePaths.Count + "\n";
                
                InfoPanel panel = new InfoPanel(mat, AssetPreview.GetMiniTypeThumbnail(typeof(Material)), "Material", info, PjktGraphics.GraphicColors["Material"]);
                
                //add a scrollview of the textures to the infoPanel
                ScrollView textureScrollView = new ScrollView();
                textureScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
                textureScrollView.contentContainer.style.flexWrap = Wrap.Wrap;
                
                foreach (string path in texturePaths)
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (texture == null) continue;
                    VisualElement texturePanel = new VisualElement();
                    texturePanel.style.width = 64;
                    texturePanel.style.height = 64;
                    texturePanel.style.backgroundImage = texture;
                    
                    //margin
                    texturePanel.style.marginTop = 3;
                    texturePanel.style.marginBottom = 3;
                    texturePanel.style.marginLeft = 3;
                    texturePanel.style.marginRight = 3;

                    //boarders
                    texturePanel.style.borderTopWidth = 1;
                    texturePanel.style.borderTopColor = Color.black;
                    texturePanel.style.borderBottomWidth = 1;
                    texturePanel.style.borderBottomColor = Color.black;
                    texturePanel.style.borderLeftWidth = 1;
                    texturePanel.style.borderLeftColor = Color.black;
                    texturePanel.style.borderRightWidth = 1;
                    texturePanel.style.borderRightColor = Color.black;
                    
                    textureScrollView.Add(texturePanel);
                }
                panel.AddMessage(textureScrollView);
                
                scrollView.Add(panel);
            }
        }
        
        private List<BoothError> FindShadersToUpdate()
        {
            List<BoothError> warnings = new List<BoothError>();
            
            //poi
            if (PjktPackageChecker.PackageSources.ContainsKey("com.poiyomi.toon"))
            {
                warnings.Add(new BoothError($"Poiyomi Toon Shader was found in this project. Please make sure you are using the latest version from VCC.\nCurrent Version: {PjktPackageChecker.PackageSources["com.poiyomi.toon"]}", BoothErrorType.Warning));
            }
            
            //poi pro
            if (PjktPackageChecker.PackageSources.ContainsKey("com.poiyomi.pro"))
            {
                warnings.Add(new BoothError($"Poiyomi Pro Shader was found in this project. Please use the free Poiyomi Toon shader instead.\nCurrent Version: {PjktPackageChecker.PackageSources["com.poiyomi.pro"]}", BoothErrorType.Warning));
            }
            
            //liltoon
            if (PjktPackageChecker.PackageSources.ContainsKey("jp.lilxyzw.liltoon"))
            {
                warnings.Add(new BoothError($"Liltoon Shader was found in this project. Please make sure you are using the latest version from VCC.\nCurrent Version: {PjktPackageChecker.PackageSources["jp.lilxyzw.liltoon"]}", BoothErrorType.Warning));
            }
            
            //orels
            if (PjktPackageChecker.PackageSources.ContainsKey("sh.orels.shaders"))
            {
                warnings.Add(new BoothError($"Orels Shaders were found in this project. Please make sure you are using the latest version from VCC.\nCurrent Version: {PjktPackageChecker.PackageSources["sh.orels.shaders"]}", BoothErrorType.Warning));
            }
            
            //mochi - diffrent because no vcc
            Shader mochiShader = Shader.Find("Mochie/Standard");
            if (mochiShader == null)
            {
                warnings.Add(new BoothError("Mochie Shaders were found in this project. Please make sure you are using the latest version to avoid conflicts with other booths.", BoothErrorType.Warning));
            }
            
            //silent - also different because no vcc
            Shader silentShader = Shader.Find("Silent/Filamented");
            if (silentShader == null)
            {
                warnings.Add(new BoothError("Silent Shaders were found in this project. Please make sure you are using the latest version to avoid conflicts with other booths.", BoothErrorType.Warning));
            }
            
            return warnings;
        }
    }
}
