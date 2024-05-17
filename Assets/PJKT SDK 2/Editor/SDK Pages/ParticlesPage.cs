using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class ParticlesPage : SDKPage
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
            
            //reportzzzz
            BoothStats particleSystems = BoothValidator.Report.GetStats(StatsType.ParticleSystem);
            
            //top
            infoBox = new RequirementsInfo(new BoothStats[] {particleSystems});
            topArea.Add(infoBox);
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //bottom
            foreach (ParticleSystem particleSystem in particleSystems.ComponentList)
            {
                ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                string particleInfo = $"Max Particles: {particleSystem.main.maxParticles}\n" 
                                      +$"Render Mode: {renderer.renderMode}\n"
                                      +$"Material: {renderer.sharedMaterial.name}\n"
                                      +$"Looping: {particleSystem.main.loop}\n"
                                      +$"Collisions: {particleSystem.collision.enabled}\n"
                                      +$"Trails: {particleSystem.trails.enabled}\n";
                
                InfoPanel panel = new InfoPanel(particleSystem.gameObject, AssetPreview.GetMiniTypeThumbnail(typeof(ParticleSystem)), "Particle System", particleInfo, PjktGraphics.GraphicColors["Particles"]);
                scrollView.Add(panel);
            }
        }
    }
}