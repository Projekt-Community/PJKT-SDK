using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.Udon;

namespace PJKT.SDK.Window
{
    internal class BoothValidator
    {
        public static BoothValidationReport Report;
        
        public static readonly Vector3 MaxDims = new Vector3(5, 5, 5);
        public const float MaxDimsMargin = 0.1f;
        public const int MaxFileSize = 10 * 1024 * 1024;
        public const int MaxPolygons = 25000;
        public const int MaxMaterialSlots = 4;
        public const int MaxPickups = 3;
        public const int MaxAnimators = 1;
        public const int MaxAnimations = 8;
        public const int MaxSkinnedMeshRenderers = 1;
        public const int MaxCanvas = 1;
        public const int MaxTextMeshPro = 1;
        public const int MaxParticles = 50;
        public const int MaxAvatarPedestals = 4;
        public const int MaxPortals = 1;
        public const int MaxMirrors = 0;
        public const int MaxUdonScripts = 3;
        private static readonly string[] UdonWhitelist = 
        {
            "Packages/com.pjkt.sdk/Runtime/Scripts/Toggle Gameobjects.asset",
            "Packages/com.vrchat.worlds/Samples/UdonExampleScene/UdonProgramSources/AvatarPedestal Program.asset",
            "Packages/com.pjkt.sdk/Runtime/Scripts/Toggle Animation.asset",
            "Packages/com.pjkt.sdk/Runtime/Scripts/Trigger Animation.asset",
            "Packages/com.pjkt.sdk/Runtime/Scripts/Set Int Animation.asset"
        };

        public static void GenerateReport(BoothDescriptor booth)
        {
            Report = new BoothValidationReport();
            
            // fetch all the relevant components
            Report.Animators = booth.gameObject.GetComponentsInChildren<Animator>(true).ToList();
            Report.Meshes = booth.gameObject.GetComponentsInChildren<MeshRenderer>(true).ToList();
            Report.SkinnedMeshes = booth.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true).ToList();
            Report.Particles = booth.gameObject.GetComponentsInChildren<ParticleSystem>(true).ToList();
            Report.Pickups = booth.gameObject.GetComponentsInChildren<VRCPickup>(true).ToList();
            Report.Portals = booth.gameObject.GetComponentsInChildren<VRCPortalMarker>(true).ToList();
            Report.CanvasComponents = booth.gameObject.GetComponentsInChildren<Canvas>(true).ToList();
            Report.TMProTexts = booth.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true).ToList();
            Report.UdonBehaviours = booth.gameObject.GetComponentsInChildren<UdonBehaviour>(true).ToList();
            Report.Renderers = booth.gameObject.GetComponentsInChildren<Renderer>(true).ToList();
            Report.Pickups = booth.gameObject.GetComponentsInChildren<VRCPickup>(true).ToList();
            Report.AvatarPeds = booth.gameObject.GetComponentsInChildren<VRCAvatarPedestal>(true).ToList();
            
            //get relevent files for components
            Report.Materials = FetchAllMaterials(Report.Renderers);
            Report.Textures = FetchAllTextures(Report.Materials);
            Report.AnimationClips = FetchAnimationClips(Report.Animators);
            
            //Set performance rankings
            Report.AnimatorRanking = Report.Animators.Count == MaxAnimators ? BoothPerformanceRanking.Ok : Report.Animators.Count > MaxAnimators ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.SkinnedMeshRanking = Report.SkinnedMeshes.Count == MaxSkinnedMeshRenderers ? BoothPerformanceRanking.Ok : Report.SkinnedMeshes.Count > MaxSkinnedMeshRenderers ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.AnimationRanking = Report.AnimationClips.Count == MaxAnimations ? BoothPerformanceRanking.Ok : Report.AnimationClips.Count > MaxAnimations ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.MaterialRanking = Report.Materials.Count == MaxMaterialSlots ? BoothPerformanceRanking.Ok : Report.Materials.Count > MaxMaterialSlots ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.PickupRanking = Report.Pickups.Count == MaxPickups ? BoothPerformanceRanking.Ok : Report.Pickups.Count > MaxPickups ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.PortalRanking = Report.Portals.Count == MaxPortals ? BoothPerformanceRanking.Ok : Report.Portals.Count > MaxPortals ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.AvatarPedistalRanking = Report.AvatarPeds.Count == MaxAvatarPedestals ? BoothPerformanceRanking.Ok : Report.AvatarPeds.Count > MaxAvatarPedestals ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.CanvasRanking = Report.CanvasComponents.Count == MaxCanvas ? BoothPerformanceRanking.Ok : Report.CanvasComponents.Count > MaxCanvas ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.TextRanking = Report.TMProTexts.Count == MaxTextMeshPro ? BoothPerformanceRanking.Ok : Report.TMProTexts.Count > MaxTextMeshPro ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;

            
            //strings
            Report.AnimatorString = "Animators: " + Report.Animators.Count + "/" + MaxAnimators;
            Report.SkinnedMeshString = "Skinned meshes: " + Report.SkinnedMeshes.Count + "/" + MaxSkinnedMeshRenderers;
            Report.AnimationClipString = "Animation clips: " + Report.AnimationClips.Count + "/" + MaxAnimations;
            Report.MaterialSlotString = "Materials: " + Report.Materials.Count + "/" + MaxMaterialSlots;
            Report.PickupString = "Pickups: " + Report.Pickups.Count + "/" + MaxPickups;
            Report.PortalString = "Portals: " + Report.Portals.Count + "/" + MaxPortals;
            Report.AvatarString = "Avatar Pedestals: " + Report.AvatarPeds.Count + "/" + MaxAvatarPedestals;
            Report.CanvasString = "Canvases: " + Report.CanvasComponents.Count + "/" + MaxCanvas;
            Report.TMPString = "Text: " + Report.TMProTexts.Count + "/" + MaxTextMeshPro;
            
            
            //other validation
            VerifyDims();
            VerifyFileSize();
            VerifyTriangleCount();
            VerifyParticleCount();
            VerifyUdonBehaviours();
            
            Report.Overallranking = OverallRank();

            //Debug.Log("Generated Booth Report");
        }

        private static HashSet<Material> FetchAllMaterials(List<Renderer> renderers)
        {
            //Get all materials
            HashSet<Material> materials = new HashSet<Material>();
            for (int i = 0; i < renderers.Count; i++)
            {
                //double check its a real texture
                //if (!File.Exists(AssetDatabase.GetAssetPath(renderers[i]))) continue;
                materials.UnionWith(renderers[i].sharedMaterials);
            }

            return materials;
        }

        private static HashSet<Texture> FetchAllTextures(HashSet<Material> materials)
        {
            //Build a set of all referenced textures
            HashSet<Texture> textures = new HashSet<Texture>();
            foreach (Material material in materials)
            {
                if (material == null) continue;

                //Get all texture properties
                foreach (string propertyName in material.GetTexturePropertyNames())
                {
                    //Get the texture
                    Texture texture = material.GetTexture(propertyName);

                    //double check its a real texture
                    if (!File.Exists(AssetDatabase.GetAssetPath(texture))) continue;
                    
                    //Add it to the set
                    textures.Add(texture);
                }
            }
            return textures;
        }
        
        private static HashSet<AnimationClip> FetchAnimationClips(List<Animator> animators)
        {
            HashSet<AnimationClip> animations = new HashSet<AnimationClip>();
            if (animators.Count == 0) return animations;
                
            //Get the animator asset paths
            List<string> animatorPaths = new List<string>();
            for (int i = 0; i < animators.Count; i++)
            {
                if (animators[i].runtimeAnimatorController == null) continue;
                animatorPaths.Add(AssetDatabase.GetAssetPath(animators[i].runtimeAnimatorController));
            }

            //Loop through all animators
            foreach (string path in animatorPaths)
            {
                //Get the animator controller
                UnityEditor.Animations.AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(path);

                //Loop through all layers
                foreach (UnityEditor.Animations.AnimatorControllerLayer layer in animatorController.layers)
                {
                    //Loop through all states
                    foreach (UnityEditor.Animations.ChildAnimatorState state in layer.stateMachine.states)
                    {
                        //Add the animation clip
                        animations.Add(state.state.motion as AnimationClip);
                    }
                }
            }

            return animations;
        }
        private static void VerifyDims() 
        {
            //sanity check
            if (Report.Renderers.Count == 0)
            {
                Report.BoundsString = "Renderer Bounds: No renderers found";
                return;
            }
            
            //Override the max dims with the margin
            Vector3 dims = MaxDims + (Vector3.one * MaxDimsMargin);

            //I have lost all faith in Bounds.Encapsulate
            Vector3 min = Report.Renderers[0].bounds.min;
            Vector3 max = Report.Renderers[0].bounds.max;

            //Get bounds of all renderers
            foreach (Renderer renderer in Report.Renderers) {
                Bounds bounds = renderer.bounds;
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
            }

            Vector3 size = max - min;

            //bool pass;
            if (size.x > dims.x || size.y > dims.y || size.z > dims.z) Report.BoundsRanking = BoothPerformanceRanking.Bad;
            else Report.BoundsRanking = BoothPerformanceRanking.Good;

            //Round up to 1 decimal place
            size.x = Mathf.Ceil(size.x * 10) / 10;
            size.y = Mathf.Ceil(size.y * 10) / 10;
            size.z = Mathf.Ceil(size.z * 10) / 10;

            //Return size to 1 decimal place
            Report.BoundsString = "Renderer Bounds: " + size.x.ToString("0.0") + " x " + size.y.ToString("0.0") + " x " + size.z.ToString("0.0") + " (max " + Mathf.Max(MaxDims.x, MaxDims.y, MaxDims.z).ToString("0.0") + ")";
        }
        private static void VerifyFileSize()
        {
            //Get the file size of all textures from the disk
            long totalSize = 0;
            foreach (Texture texture in Report.Textures)
            {
                //Get the path
                string path = AssetDatabase.GetAssetPath(texture);

                //Check if it's a valid path
                if (path == null || path == "")
                {
                    continue;
                }

                //Get the file info
                FileInfo fileInfo = new FileInfo(path);

                //Add the size to the total
                totalSize += fileInfo.Length;
            }

            //Format data for printing
            float totalMB = (float) totalSize / (1024f * 1024f);
            int totalMBInt = Mathf.CeilToInt(totalMB);
            const int maxMBInt = MaxFileSize / (1024 * 1024);

            //set performance and string
            Report.TextureRanking = totalMB == maxMBInt ? BoothPerformanceRanking.Ok : totalMBInt > maxMBInt ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.TextureString = "Texture size on disk (MB): " + totalMB.ToString("0.0") + " / " + maxMBInt + " MB";
        }
        private static void VerifyTriangleCount()
        {
            long triangles = 0;

            foreach (Renderer renderer in Report.Renderers)
            {
                //Check every mesh
                MeshFilter[] meshFilters = renderer.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    //Add the number of triangles
                    triangles += meshFilter.sharedMesh.triangles.Length / 3;
                }

                //Check every skinned mesh
                foreach (SkinnedMeshRenderer skinnedMeshRenderer in Report.SkinnedMeshes)
                {
                    //Add the number of triangles
                    triangles += skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
                }
            }

            Report.TriCountRanking = triangles == MaxPolygons ? BoothPerformanceRanking.Ok : triangles > MaxPolygons ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
            Report.TriCountString = "Mesh Triangles: " + triangles + "/" + MaxPolygons;
        }
        private static void VerifyParticleCount()
        {
            //Count the number of particles
            int particleCount = 0;
            foreach (ParticleSystem particleSystem in Report.Particles)
            {
                particleCount += particleSystem.main.maxParticles;
            }
            Report.ParticleString = "Particle Systems: " + particleCount + "/" + MaxParticles;
            
            //Verify no collisions
            foreach (ParticleSystem particleSystem in Report.Particles)
            {
                if (particleSystem.collision.enabled)
                {
                    Report.ParticleString += " (Collision Enabled)";
                    Report.ParticlRanking = BoothPerformanceRanking.Error;
                    return;
                }
            }
            Report.ParticlRanking = particleCount == MaxParticles ? BoothPerformanceRanking.Ok : particleCount > MaxParticles ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
        }
        private static void VerifyUdonBehaviours()
        {
            List<UdonBehaviour> disallowedScripts = new List<UdonBehaviour>(Report.UdonBehaviours);

            //Check the path against the whitelist of allowed scripts
            foreach (UdonBehaviour udonScript in Report.UdonBehaviours)
            {
                //check if null
                if (udonScript.programSource == null)
                {
                    disallowedScripts.Remove(udonScript);
                    continue;
                }
                
                //Get the program source
                AbstractUdonProgramSource programSource = udonScript.programSource;
                string path = AssetDatabase.GetAssetPath(programSource);

                //Check if the path is in the whitelist
                foreach (string allowedItem in UdonWhitelist)
                {
                    if (path.Equals(allowedItem))
                    {
                        disallowedScripts.Remove(udonScript);
                        break;
                    }
                }
            }

            Report.UdonString = "UdonBehaviours: " + Report.UdonBehaviours.Count + "/" + MaxUdonScripts;
            
            if (disallowedScripts.Count != 0)
            {
                Report.UdonString += " (Disallowed scripts)";
                Report.UdonBehaviourRanking = BoothPerformanceRanking.Error;
                Debug.Log("Disallowed udon script "+ disallowedScripts.Count + AssetDatabase.GetAssetPath(disallowedScripts[0]));
                return;
            }
            Report.UdonBehaviourRanking = Report.UdonBehaviours.Count == MaxUdonScripts ? BoothPerformanceRanking.Ok : Report.UdonBehaviours.Count > MaxUdonScripts ? BoothPerformanceRanking.Bad : BoothPerformanceRanking.Good;
        }

        private static BoothPerformanceRanking OverallRank()
        {
            //not sure of a better way to do this but eeh
            List<BoothPerformanceRanking> allRanks = new List<BoothPerformanceRanking>();
            allRanks.Add(Report.BoundsRanking);
            allRanks.Add(Report.TriCountRanking);
            allRanks.Add(Report.SkinnedMeshRanking);
            allRanks.Add(Report.MaterialRanking);
            allRanks.Add(Report.TextureRanking);
            allRanks.Add(Report.PickupRanking);
            allRanks.Add(Report.AnimatorRanking);
            allRanks.Add(Report.AnimationRanking);
            allRanks.Add(Report.CanvasRanking);
            allRanks.Add(Report.TextRanking);
            allRanks.Add(Report.ParticlRanking);
            allRanks.Add(Report.AvatarPedistalRanking);
            allRanks.Add(Report.PortalRanking);
            allRanks.Add(Report.UdonBehaviourRanking);

            BoothPerformanceRanking overallrank = BoothPerformanceRanking.NotApplicable;
            foreach (BoothPerformanceRanking rank in allRanks)
            {
                if (rank < overallrank) overallrank = rank;
            }

            return overallrank;
        }
    }
}