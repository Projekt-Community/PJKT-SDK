using System.Collections.Generic;
using PJKT.SDK.Window;
using TMPro;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR.WSA;
using VRC.SDK3.Components;
using VRC.Udon;

namespace PJKT.SDK
{
    internal class BoothTreeItem : TreeViewItem
    {
        public Object ObjectRef;
        public BoothPerformanceRanking PerfRank;

        public BoothTreeItem(Object obj, BoothPerformanceRanking rank = BoothPerformanceRanking.NotApplicable)
        {
            ObjectRef = obj;
            PerfRank = rank;
        }
    }
    internal class BoothValidationTreeView : TreeView
    {
        private List<TreeViewItem> boothTreeItems;
        
        public BoothValidationTreeView(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            showAlternatingRowBackgrounds = true;
            rowHeight = 20;
            showBorder = true;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            BoothTreeItem item = (BoothTreeItem)args.item;

            Rect rect = args.GetCellRect(0);
            rect.x += (16 + (16 * args.item.depth));
            if (item.depth == 0) EditorGUI.LabelField(rect, item.displayName);
            else
            {
                //GUI.Box(rect, new GUIContent(item.icon, item.ObjectRef.name));
                EditorGUI.LabelField(rect, new GUIContent(){image = item.icon, text = item.displayName});
                return;
            }

            rect = args.GetCellRect(1);
            GUI.Box(rect, new GUIContent(GetPerformanceIcon(item.PerfRank), item.PerfRank.ToString()));
        }
        
        //the root of the entire tree, its always hidden
        protected override TreeViewItem BuildRoot()
        {
            int idnum = 1;
            var root = new BoothTreeItem(null)
            {
                id = idnum,
                displayName = "Root",
                depth = -1
            };
            idnum++;

            boothTreeItems = new List<TreeViewItem>();
            
            // Max dims 5x5x5
            BoothTreeItem bounds = new BoothTreeItem(null, BoothValidator.Report.BoundsRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.BoundsString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.BoundsRanking)
            };
            idnum++;
            boothTreeItems.Add(bounds);
            
            // Max file size 10 MB uncompressed 
            BoothTreeItem fileSize = new BoothTreeItem(null, BoothValidator.Report.TextureRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.TextureString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.TextureRanking)
            };
            idnum++;
            boothTreeItems.Add(fileSize);
            
            foreach (Texture texture in BoothValidator.Report.Textures)
            {
                BoothTreeItem tex = new BoothTreeItem(texture)
                {
                    id = idnum,
                    displayName = texture.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(Texture)),
                };
                idnum++;
                fileSize.AddChild(tex);
            }
            
            // 1 Skinned mesh renderer 
            BoothTreeItem skinedMeshCount = new BoothTreeItem(null, BoothValidator.Report.SkinnedMeshRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.SkinnedMeshString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.SkinnedMeshRanking)
            };
            idnum++;
            boothTreeItems.Add(skinedMeshCount);
            
            // 25K polygon max 
            BoothTreeItem triCount = new BoothTreeItem(null, BoothValidator.Report.TriCountRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.TriCountString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.TriCountRanking),
            };
            idnum++;
            boothTreeItems.Add(triCount);

            for (int i = 0; i < BoothValidator.Report.Meshes.Count; i++)
            {
                BoothTreeItem mesh = new BoothTreeItem(BoothValidator.Report.Meshes[i])
                {
                    id = idnum,
                    displayName = BoothValidator.Report.Meshes[i].name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(MeshRenderer)),
                };
                idnum++;
                triCount.AddChild(mesh);
            }
            for (int i = 0; i < BoothValidator.Report.SkinnedMeshes.Count; i++)
            {
                BoothTreeItem skinnedmeshTriCount = new BoothTreeItem(BoothValidator.Report.SkinnedMeshes[i])
                {
                    id = idnum,
                    displayName = BoothValidator.Report.SkinnedMeshes[i].name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(SkinnedMeshRenderer)),
                };
                idnum++;
                triCount.AddChild(skinnedmeshTriCount);
                skinedMeshCount.AddChild(skinnedmeshTriCount);
            }

            // 4 Material slots 
            BoothTreeItem matCount = new BoothTreeItem(null, BoothValidator.Report.MaterialRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.MaterialSlotString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.MaterialRanking)
            };
            idnum++;
            boothTreeItems.Add(matCount);

            foreach (Material material in BoothValidator.Report.Materials)
            {
                BoothTreeItem mat = new BoothTreeItem(material)
                {
                    id = idnum,
                    displayName = material.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(Material)),
                };
                idnum++;
                matCount.AddChild(mat);
            }
            
            // 3 Pickups (these count towards your maximum material slots!!) 
            BoothTreeItem pickupCount = new BoothTreeItem(null, BoothValidator.Report.PickupRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.PickupString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.PickupRanking)
                
            };
            idnum++;
            boothTreeItems.Add(pickupCount);

            foreach (VRCPickup pickup in BoothValidator.Report.Pickups)
            {
                BoothTreeItem pick = new BoothTreeItem(pickup)
                {
                    id = idnum,
                    displayName = pickup.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(AudioDistortionFilter)),
                };
                idnum++;
                pickupCount.AddChild(pick);
            }
            
            // 1 Animator 
            BoothTreeItem animatorCount = new BoothTreeItem(null, BoothValidator.Report.AnimatorRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.AnimatorString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.AnimatorRanking)
            };
            idnum++;
            boothTreeItems.Add(animatorCount);
            
            foreach (Animator animator in BoothValidator.Report.Animators)
            {
                BoothTreeItem animcomp = new BoothTreeItem(animator)
                {
                    id = idnum,
                    displayName = animator.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(Animator)),
                };
                idnum++;
                animatorCount.AddChild(animcomp);
            }
            
            // 8 Animations 
            BoothTreeItem animationCount = new BoothTreeItem(null, BoothValidator.Report.AnimationRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.AnimationClipString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.AnimationRanking)
                //icon = AssetPreview.GetMiniTypeThumbnail(typeof(AnimationClip)),
            };
            idnum++;
            boothTreeItems.Add(animationCount);
            
            foreach (AnimationClip clip in BoothValidator.Report.AnimationClips)
            {
                BoothTreeItem animclip = new BoothTreeItem(clip)
                {
                    id = idnum,
                    displayName = clip.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(AnimationClip)),
                };
                idnum++;
                animationCount.AddChild(animclip);
            }

            // 1 canvas, 1 TextMeshPro (don't use UI text) 
            BoothTreeItem textCount = new BoothTreeItem(null, BoothValidator.Report.TextRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.TMPString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.TextRanking)
            };
            idnum++;
            boothTreeItems.Add(textCount);
            
            foreach (TMP_Text text in BoothValidator.Report.TMProTexts)
            {
                BoothTreeItem tmp = new BoothTreeItem(text)
                {
                    id = idnum,
                    displayName = text.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(TextMesh)),
                };
                idnum++;
                textCount.AddChild(tmp);
            }
            
            // Particles are allowed (Max particle count is 50, no collision, counts towards material count!!) 
            BoothTreeItem particleCount = new BoothTreeItem(null, BoothValidator.Report.ParticlRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.ParticleString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.ParticlRanking)
            };
            idnum++;
            boothTreeItems.Add(particleCount);
            
            foreach (ParticleSystem system in BoothValidator.Report.Particles)
            {
                BoothTreeItem particlesystem = new BoothTreeItem(system)
                {
                    id = idnum,
                    displayName = system.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(ParticleSystem)),
                };
                idnum++;
                particleCount.AddChild(particlesystem);
            }
            
            // 4 Avatar pedestals (Off by default, can be turned on by the provided button) 
            BoothTreeItem avatarCount = new BoothTreeItem(null, BoothValidator.Report.AvatarPedistalRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.AvatarString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.AvatarPedistalRanking)
            };
            idnum++;
            boothTreeItems.Add(avatarCount);
            
            foreach (VRCAvatarPedestal pedistal in BoothValidator.Report.AvatarPeds)
            {
                BoothTreeItem ped = new BoothTreeItem(pedistal)
                {
                    id = idnum,
                    displayName = pedistal.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(Avatar)),
                };
                idnum++;
                avatarCount.AddChild(ped);
            }
            
            // Portals allowed (Off by default, can be turned on by the provided button) 
            BoothTreeItem portalCount = new BoothTreeItem(null, BoothValidator.Report.PortalRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.PortalString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.PortalRanking)
            };
            idnum++;
            boothTreeItems.Add(portalCount);
            
            foreach (VRCPortalMarker portal in BoothValidator.Report.Portals)
            {
                BoothTreeItem spaceHole = new BoothTreeItem(portal)
                {
                    id = idnum,
                    displayName = portal.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(WorldAnchor)),
                };
                idnum++;
                portalCount.AddChild(spaceHole);
            }
            
            // whitelisted udon components
            BoothTreeItem udonCount = new BoothTreeItem(null, BoothValidator.Report.UdonBehaviourRanking)
            {
                id = idnum,
                displayName = BoothValidator.Report.UdonString,
                depth = 0,
                icon = GetPerformanceIcon(BoothValidator.Report.UdonBehaviourRanking)
            };
            idnum++;
            boothTreeItems.Add(udonCount);
            
            foreach (UdonBehaviour script in BoothValidator.Report.UdonBehaviours)
            {
                BoothTreeItem udon = new BoothTreeItem(script)
                {
                    id = idnum,
                    displayName = script.gameObject.name,
                    depth = 2,
                    icon = AssetPreview.GetMiniTypeThumbnail(typeof(Tilemap)),
                };
                idnum++;
                udonCount.AddChild(udon);
            }
            
            SetupParentsAndChildrenFromDepths(root, boothTreeItems);
            return root;
        }

        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);
            var obj = (BoothTreeItem) FindItem(id, rootItem);
            if (obj.ObjectRef != null)
            {
                EditorGUIUtility.PingObject(obj.ObjectRef);
                Selection.activeObject = obj.ObjectRef;
            }
        }

        private Texture2D excelentIcon;
        private Texture2D goodIcon;
        private Texture2D mediumIcon;
        private Texture2D poorIcon;
        private Texture2D veryPoorIcon;
        
        public void LoadTextures()
        {
            //Debug.Log("Loading textures");
            excelentIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.vrchat.base/Runtime/VRCSDK/Dependencies/VRChat/Resources/PerformanceIcons/Perf_Great_32.png");
            goodIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.vrchat.base/Runtime/VRCSDK/Dependencies/VRChat/Resources/PerformanceIcons/Perf_Good_32.png");
            mediumIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.vrchat.base/Runtime/VRCSDK/Dependencies/VRChat/Resources/PerformanceIcons/Perf_Medium_32.png");
            poorIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.vrchat.base/Runtime/VRCSDK/Dependencies/VRChat/Resources/PerformanceIcons/Perf_Poor_32.png");
            veryPoorIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.vrchat.base/Runtime/VRCSDK/Dependencies/VRChat/Resources/PerformanceIcons/Perf_Horrible_32.png");
        }
        private Texture2D GetPerformanceIcon(BoothPerformanceRanking perfRank)
        {
            switch (perfRank)
            {
                case BoothPerformanceRanking.NotApplicable:
                    return excelentIcon;
                case BoothPerformanceRanking.Good:
                    return goodIcon;
                case BoothPerformanceRanking.Ok:
                    return mediumIcon;
                case BoothPerformanceRanking.Bad:
                    return poorIcon;
                case BoothPerformanceRanking.Error:
                    return veryPoorIcon;
            }

            return excelentIcon;
        }
    }
}