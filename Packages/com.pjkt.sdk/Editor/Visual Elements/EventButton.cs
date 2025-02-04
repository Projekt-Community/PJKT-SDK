using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class EventButton : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<EventButton> { }
        private const string uxmlPath = "Packages/com.pjkt.sdk/Editor/Visual Elements/EventButton.uxml";
        public EventButton() { }
        
        public Project Projekt { get; private set; }
        
        //titlebar
        public VisualElement titleBar => this.Q<VisualElement>("TitleBar");
        private VisualElement background => this.Q<VisualElement>("Background");
        private VisualElement middleLogo => this.Q<VisualElement>("MiddleLogo");
        
        //requirements
        private VisualElement requirementsContainer => this.Q<VisualElement>("Requirements_Info");
        private VisualElement requirementsColumn1 => this.Q<VisualElement>("Requirements_Column_1");
        private VisualElement requirementsColumn2 => this.Q<VisualElement>("Requirements_Column_2");

        private BoothRequirements requirements;
        
        public EventButton(Project pjktEvent)
        {
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            asset.CloneTree(this);
            Projekt = pjktEvent;
            if (Projekt != null) FetchEventLogo();
            requirements = pjktEvent.booth_requirements;
            background.style.backgroundImage = PjktGraphics.GetRandomPaintSplat();
            titleBar.tooltip = pjktEvent.name;
        }

        private async void FetchEventLogo()
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(Projekt.Logo.path);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) return;
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            middleLogo.style.backgroundImage = tex; Debug.Log("got the img");
        }
        
        public void SelectEvent()
        {
            //assess the booth
            PjktEventManager.SelectedProjekt = Projekt;
            BoothDescriptor._maxBounds = new Vector3(requirements.MaxDims[0], requirements.MaxDims[2], requirements.MaxDims[1]);
            BoothValidator.Requirements = requirements;
            BoothValidator.GenerateReport();
            
            //create a requirement category for each requirement in the booth requirements
            if (BoothValidator.SelectedBooth == null || BoothValidator.Report == null) SetRequirementsNoBooth();
            else SetRequirements();
        }

        private void SetRequirementsNoBooth()
        {
            requirementsColumn1.Clear();
            requirementsColumn2.Clear();
            
            Texture2D icon = (Texture2D)EditorGUIUtility.IconContent("d__Help@2x").image;
            
            //column 1
            requirementsColumn1.Add(new RequirementCategory("Max Triangles: " + requirements.MaxTriangles, icon));
            requirementsColumn1.Add(new RequirementCategory("Max Meshes: " + requirements.MaxStaticMeshes, icon));
            requirementsColumn1.Add(new RequirementCategory("Max Skinned Meshes: " + requirements.MaxSkinnedMeshRenderers, icon));
            requirementsColumn1.Add(new RequirementCategory("Max Materials: " + requirements.MaxMaterial, icon));
            requirementsColumn1.Add(new RequirementCategory($"Max Dimensions: ({requirements.MaxDims[0]}, {requirements.MaxDims[1]}, {requirements.MaxDims[2]})", icon));
            requirementsColumn1.Add(new RequirementCategory("Max Animators: " + requirements.MaxAnimators, icon));
            requirementsColumn1.Add(new RequirementCategory("Max Animations: " + requirements.MaxAnimations, icon));
            requirementsColumn1.Add(new RequirementCategory("Max Particles: " + requirements.MaxParticles, icon));
            
            //column 2
            requirementsColumn2.Add(new RequirementCategory("Max Filesize: " + BoothValidator.FormatSize(requirements.MaxFileSize *1024*1024), icon));
            requirementsColumn2.Add(new RequirementCategory("Max Vram: " + BoothValidator.FormatSize(requirements.MaxVram * 1024 * 1024), icon));
            requirementsColumn2.Add(new RequirementCategory("Max Text: " + requirements.MaxTextMeshPro, icon));
            requirementsColumn2.Add(new RequirementCategory("Max Pickups: " + requirements.MaxPickups, icon));
            requirementsColumn2.Add(new RequirementCategory("Max Avatar Pedistals: " + requirements.MaxAvatarPedestals, icon));
            requirementsColumn2.Add(new RequirementCategory("Max Portals: " + requirements.MaxPortals, icon));
            requirementsColumn2.Add(new RequirementCategory("Max Udon Behaviours: " + requirements.MaxUdonScripts, icon));
        }
        private void SetRequirements()
        {
            requirementsColumn1.Clear();
            requirementsColumn2.Clear();
            
            BoothValidationReport report = BoothValidator.Report;

            Texture2D goodPerfIcon = (Texture2D)EditorGUIUtility.IconContent("P4_CheckOutRemote@2x").image;
            Texture2D badPerfIcon = (Texture2D)EditorGUIUtility.IconContent("P4_DeletedLocal@2x").image;
            
            //column 1 //god bless copilot for this
            BoothStats tricount = report.GetStats(StatsType.TriCount);
            requirementsColumn1.Add(new RequirementCategory("Max Triangles: " + requirements.MaxTriangles, (int)tricount.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats staticMeshes = report.GetStats(StatsType.Mesh);
            requirementsColumn1.Add(new RequirementCategory("Max Meshes: " + requirements.MaxStaticMeshes, (int)staticMeshes.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats skinnedMeshes = report.GetStats(StatsType.SkinnedMesh);
            requirementsColumn1.Add(new RequirementCategory("Max Skinned Meshes: " + requirements.MaxSkinnedMeshRenderers, (int)skinnedMeshes.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats materials = report.GetStats(StatsType.Materials);
            requirementsColumn1.Add(new RequirementCategory("Max Materials: " + requirements.MaxMaterial, (int)materials.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats dims = report.GetStats(StatsType.Bounds);
            requirementsColumn1.Add(new RequirementCategory($"Max Dimensions: ({requirements.MaxDims[0]}, {requirements.MaxDims[1]}, {requirements.MaxDims[2]})", (int)dims.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats animators = report.GetStats(StatsType.Animators);
            requirementsColumn1.Add(new RequirementCategory("Max Animators: " + requirements.MaxAnimators, (int)animators.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats animations = report.GetStats(StatsType.AnimationClips);
            requirementsColumn1.Add(new RequirementCategory("Max Animations: " + requirements.MaxAnimations, (int)animations.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats particles = report.GetStats(StatsType.ParticleSystem);
            requirementsColumn1.Add(new RequirementCategory("Max Particles: " + requirements.MaxParticles, (int)particles.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            
            //column 2
            BoothStats filesize = report.GetStats(StatsType.FileSize);
            requirementsColumn2.Add(new RequirementCategory("Max Filesize: " + BoothValidator.FormatSize(requirements.MaxFileSize*1024*1024), (int)filesize.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats vram = report.GetStats(StatsType.Vram);
            requirementsColumn2.Add(new RequirementCategory("Max Vram: " + BoothValidator.FormatSize(requirements.MaxVram * 1024 * 1024), (int)vram.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats text = report.GetStats(StatsType.TMProTexts);
            requirementsColumn2.Add(new RequirementCategory("Max Text: " + requirements.MaxTextMeshPro, (int)text.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats pickups = report.GetStats(StatsType.Pickups);
            requirementsColumn2.Add(new RequirementCategory("Max Pickups: " + requirements.MaxPickups, (int)pickups.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats avatars = report.GetStats(StatsType.AvatarPeds);
            requirementsColumn2.Add(new RequirementCategory("Max Avatar Pedistals: " + requirements.MaxAvatarPedestals, (int)avatars.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats portals = report.GetStats(StatsType.Portals);
            requirementsColumn2.Add(new RequirementCategory("Max Portals: " + requirements.MaxPortals, (int)portals.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
            BoothStats udon = report.GetStats(StatsType.UdonBehaviours);
            requirementsColumn2.Add(new RequirementCategory("Max Udon Behaviours: " + requirements.MaxUdonScripts, (int)udon.PerformanceRank >= 1 ? goodPerfIcon : badPerfIcon));
        }

        public void ShowRequirements(bool shown)
        {
            requirementsContainer.style.display = shown ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}