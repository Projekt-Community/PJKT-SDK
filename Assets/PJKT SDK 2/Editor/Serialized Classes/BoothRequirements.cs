using System;

namespace PJKT.SDK2
{
    [Serializable]
    public class BoothRequirements
    {
        public BoothRequirements()
        {
            MaxTriangles = 0;
            MaxStaticMeshes = 1;
            MaxMaterial = 0;
            MaxDims = new float[3];
            MaxDimsMargin = 0;
            MaxFileSize = 0;
            MaxVram = 0;
            MaxSkinnedMeshRenderers = 0;
            MaxAnimators = 0;
            MaxAnimations = 0;
            MaxTextMeshPro = 0;
            MaxParticles = 0;
            MaxPickups = 0;
            MaxAvatarPedestals = 0;
            MaxPortals = 0;
            MaxMirrors = 0;
            MaxUdonScripts = 0;
            UdonWhitelist = new string[0];
        }
        public BoothRequirements(int maxTriangles, int maxStaticMeshes, int maxMaterial, float[] maxDims, float maxDimsMargin, int maxFileSize, int maxVram, int maxSkinnedMeshRenderers, int maxAnimators, int maxAnimations, int maxTextMeshPro, int maxParticles, int maxPickups, int maxAvatarPedestals, int maxPortals, int maxMirrors, int maxUdonScripts, string[] udonWhitelist)
        {
            MaxTriangles = maxTriangles;
            MaxStaticMeshes = maxStaticMeshes;
            MaxMaterial = maxMaterial;
            MaxDims = maxDims;
            MaxDimsMargin = maxDimsMargin;
            MaxFileSize = maxFileSize;
            MaxVram = maxVram;
            MaxSkinnedMeshRenderers = maxSkinnedMeshRenderers;
            MaxAnimators = maxAnimators;
            MaxAnimations = maxAnimations;
            MaxTextMeshPro = maxTextMeshPro;
            MaxParticles = maxParticles;
            MaxPickups = maxPickups;
            MaxAvatarPedestals = maxAvatarPedestals;
            MaxPortals = maxPortals;
            MaxMirrors = maxMirrors;
            MaxUdonScripts = maxUdonScripts;
            UdonWhitelist = udonWhitelist;
        }

        public float[] MaxDims;
        public int MaxVram;
        public int MaxMirrors;
        public int MaxPickups;
        public int MaxPortals;
        public int MaxFileSize;
        public int MaxMaterial;
        public int MaxAnimators;
        public int MaxParticles;
        public int MaxTriangles;
        public int MaxAnimations;
        public float MaxDimsMargin;
        public string[] UdonWhitelist;
        public int MaxTextMeshPro;
        public int MaxUdonScripts;
        public int MaxAvatarPedestals;
        public int MaxSkinnedMeshRenderers;
        public int MaxStaticMeshes;

        /*
        //model
        public readonly int MaxTriangles;
        public readonly int MaxStaticMeshes;
        public readonly int MaxMaterial;
        public readonly float[] MaxDims;
        public readonly float MaxDimsMargin;
        
        //size
        public readonly int MaxFileSize;
        public readonly int MaxVram;
        
        //animations
        public readonly int MaxSkinnedMeshRenderers;
        public readonly int MaxAnimators;
        public readonly int MaxAnimations;
        
        //TMP
        public readonly int MaxTextMeshPro;
        
        //particles
        public readonly int MaxParticles;
        
        //vrchat stuff
        public readonly int MaxPickups;
        public readonly int MaxAvatarPedestals;
        public readonly int MaxPortals;
        public readonly int MaxMirrors;
        public readonly int MaxUdonScripts;
        public readonly string[] UdonWhitelist;*/
    }
}
