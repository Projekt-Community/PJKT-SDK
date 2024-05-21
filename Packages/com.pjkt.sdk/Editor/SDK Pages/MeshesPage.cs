using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class MeshesPage : SDKPage
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
            
            //get the report
            BoothStats meshes = BoothValidator.Report.GetStats(StatsType.Mesh);
            BoothStats skinnedMeshes = BoothValidator.Report.GetStats(StatsType.SkinnedMesh);
            BoothStats particleMeshes = BoothValidator.Report.GetStats(StatsType.ParticleMesh);

            //create the top info area
            infoBox = new RequirementsInfo(new BoothStats[] {meshes, skinnedMeshes, particleMeshes});
            topArea.Add(infoBox);   
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //fill out info box
            foreach (MeshAsset asset in meshes.ComponentList) CreateInfoPannel(asset);
            foreach (MeshAsset asset in skinnedMeshes.ComponentList) CreateInfoPannel(asset);
            foreach (MeshAsset asset in particleMeshes.ComponentList) CreateInfoPannel(asset);
        }

        private void CreateInfoPannel(MeshAsset asset)
        {
            Color color = PjktGraphics.GraphicColors["Mesh"];
            string meshinfo = $"Mesh: {asset.Name} \n"
                              + $"Triangles: {asset.TriCount} \n"
                              + $"Material Slots: {asset.MaterialSlots} \n"
                              + "Vram: " + BoothValidator.FormatSize(asset.VramSize);
            if (asset.Type == MeshType.SkinnedMesh)
            {
                color = PjktGraphics.GraphicColors["Settings"];
                meshinfo += $"\nBlend Shapes: {asset.BlendShapes}";
            }
            else if (asset.Type == MeshType.ParticleMesh) color = PjktGraphics.GraphicColors["Particles"];

            InfoPanel panel = new InfoPanel(asset.ObjectReference, (Texture2D)asset.Icon, asset.Type.ToString(), meshinfo, color);

            //add the panel to the page
            scrollView.Add(panel);
        }

        public override void OnTabDisable()
        {
            style.display = DisplayStyle.None;
            scrollView.Clear();
        }
    }
}