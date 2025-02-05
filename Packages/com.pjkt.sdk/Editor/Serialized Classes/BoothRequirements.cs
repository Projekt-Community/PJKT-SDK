using System;

namespace PJKT.SDK2
{
    [Serializable]
    public class BoothRequirements
    {
        public BoothRequirements()
        {
            MaxTriangles = 0;
            MaxStaticMeshes = 0;
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
            MaxBuildSize = 0;
        }
        public BoothRequirements(int maxTriangles, int maxStaticMeshes, int maxMaterial, float[] maxDims, float maxDimsMargin, int maxFileSize, int maxVram, int maxSkinnedMeshRenderers, int maxAnimators, int maxAnimations, int maxTextMeshPro, int maxParticles, int maxPickups, int maxAvatarPedestals, int maxPortals, int maxMirrors, int maxUdonScripts, string[] udonWhitelist, int maxBuildSize)
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
            MaxBuildSize = maxBuildSize;
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
        public int MaxBuildSize;
    }
}
