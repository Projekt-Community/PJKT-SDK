using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.Udon;

namespace PJKT.SDK
{
    public enum BoothPerformanceRanking
    {
        NotApplicable = 3,
        Good = 2,
        Ok = 1,
        Bad = 0,
        Error = -1
    }
    public class BoothValidationReport
    {
        public BoothPerformanceRanking Overallranking = BoothPerformanceRanking.Error;
        
        public Vector3 Bounds;
        public string BoundsString;
        public BoothPerformanceRanking BoundsRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<MeshRenderer> Meshes = new List<MeshRenderer>();
        public string TriCountString;
        public BoothPerformanceRanking TriCountRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<SkinnedMeshRenderer> SkinnedMeshes = new List<SkinnedMeshRenderer>();
        public string SkinnedMeshString;
        public BoothPerformanceRanking SkinnedMeshRanking = BoothPerformanceRanking.NotApplicable;

        public List<Renderer> Renderers = new List<Renderer>();

        public HashSet<Material> Materials = new HashSet<Material>();
        public string MaterialSlotString;
        public BoothPerformanceRanking MaterialRanking = BoothPerformanceRanking.NotApplicable;
        
        public HashSet<Texture> Textures = new HashSet<Texture>();
        public string TextureString;
        public BoothPerformanceRanking TextureRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<VRCPickup> Pickups = new List<VRCPickup>();
        public string PickupString;
        public BoothPerformanceRanking PickupRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<Animator> Animators = new List<Animator>();
        public string AnimatorString;
        public BoothPerformanceRanking AnimatorRanking = BoothPerformanceRanking.NotApplicable;
        
        public HashSet<AnimationClip> AnimationClips = new HashSet<AnimationClip>();
        public string AnimationClipString;
        public BoothPerformanceRanking AnimationRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<Canvas> CanvasComponents = new List<Canvas>();
        public string CanvasString;
        public BoothPerformanceRanking CanvasRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<TextMeshProUGUI> TMProTexts = new List<TextMeshProUGUI>();
        public string TMPString;
        public BoothPerformanceRanking TextRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<ParticleSystem> Particles = new List<ParticleSystem>();
        public string ParticleString;
        public BoothPerformanceRanking ParticlRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<VRCAvatarPedestal> AvatarPeds = new List<VRCAvatarPedestal>();
        public string AvatarString;
        public BoothPerformanceRanking AvatarPedistalRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<VRCPortalMarker> Portals = new List<VRCPortalMarker>();
        public string PortalString;
        public BoothPerformanceRanking PortalRanking = BoothPerformanceRanking.NotApplicable;
        
        public List<UdonBehaviour> UdonBehaviours = new List<UdonBehaviour>();
        public string UdonString;
        public BoothPerformanceRanking UdonBehaviourRanking = BoothPerformanceRanking.NotApplicable;
    }
}