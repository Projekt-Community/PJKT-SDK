using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace PJKT.SDK2
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
        public readonly List<BoothStats> Stats = new List<BoothStats>();

        public BoothStats GetStats(StatsType statsType)
        {
            foreach (BoothStats stat in Stats)
            {
                if (stat.Type == statsType)
                {
                    return stat;
                }
            }
            //Debug.LogWarning("BoothStats not found for " + statsType);
            return null;
        }
    }
    
    public enum MeshType
    {
        StaticMesh,
        SkinnedMesh,
        TextMeshPro,
        ParticleMesh,
    }

    public enum StatsType
    {
        Bounds,
        FileSize,
        TriCount,
        Mesh,
        SkinnedMesh,
        ParticleMesh,
        Vram,
        Materials,
        MaterialSlots,
        Textures,
        Pickups,
        Animators,
        AnimationClips,
        TMProTexts,
        ParticleSystem,
        AvatarPeds,
        Portals,
        UdonBehaviours,
    }

    public class MeshAsset
    {
        //data to display
        public string Name;
        public int TriCount;
        public MeshType Type;
        public int BlendShapes;
        public int MaterialSlots;
        public long VramSize;


        //for internal use
        public GameObject ObjectReference;
        public Texture Icon;
    }
    
    public class TextureInfo
    {
        public Texture texture;
        public string name;
        public string filetype;
        public int[] pixelSize;
        public int importedSize;
        public long vRamSize;
        public TextureImporter importer;
        public List<Material> materials;
    }

    public class UdonInfo
    {
        public UdonBehaviour Behaviour;
        public string ProgramSource;
        public bool Allowed;
        public Networking.SyncType SyncType;
    }
    
    public class AnimatorInfo
    {
        public Animator Animator;
        public AnimatorController Controller;
        public List<AnimationClip> Clips;
        public int Layers;
        public int States;
        public int AnyStateTransitions;
        public List<Tuple<string, AnimatorControllerParameterType>> Parameters;
    }
}