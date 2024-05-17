using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PJKT.SDK2
{
    public static class PjktGraphics
    {
        private static Dictionary<string, string> _graphicPaths = new Dictionary<string, string>()
        {
            {"PjktLogo", "Assets/PJKT SDK 2/Editor/Graphics/pjktLogo.png"},
            {"FestLogo", "Assets/PJKT SDK 2/Editor/Graphics/PJKTFESTLogo.png"},
            {"Animation", "Assets/PJKT SDK 2/Editor/Graphics/Animation.png"},
            {"Booths", "Assets/PJKT SDK 2/Editor/Graphics/Booths.png"},
            {"EventTicket", "Assets/PJKT SDK 2/Editor/Graphics/EventTicket.png"},
            {"Material", "Assets/PJKT SDK 2/Editor/Graphics/Material.png"},
            {"Mesh", "Assets/PJKT SDK 2/Editor/Graphics/Mesh.png"},
            {"PaintSplat1", "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat1.png"},
            {"PaintSplat2", "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat2.png"},
            {"PaintSplat3", "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat3.png"},
            {"PaintSplat4", "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat4.png"},
            {"paintLeft", "Assets/PJKT SDK 2/Editor/Graphics/paintLeft.png"},
            {"paintRight", "Assets/PJKT SDK 2/Editor/Graphics/paintRight.png"},
            {"Particles", "Assets/PJKT SDK 2/Editor/Graphics/Particles.png"},
            {"Settings", "Assets/PJKT SDK 2/Editor/Graphics/Settings.png"},
            {"Text", "Assets/PJKT SDK 2/Editor/Graphics/Text.png"},
            {"Texture", "Assets/PJKT SDK 2/Editor/Graphics/Texture.png"},
            {"Avatar", "Assets/PJKT SDK 2/Editor/Graphics/AvatarPedistalIcon.png"},
            {"UdonBehaviour", "Assets/PJKT SDK 2/Editor/Graphics/UdonBehaviourIcon.png"},
            {"Portal", "Assets/PJKT SDK 2/Editor/Graphics/PortalIcon.png"},
            {"Pickup", "Assets/PJKT SDK 2/Editor/Graphics/PickupIcon.png"},
        };
        
        public static readonly Dictionary<string, Color> GraphicColors = new Dictionary<string, Color>()
        {
            {"PjktLogo", new Color(0.5f, 0.5f, 0.5f)},
            {"Animation", new Color(0.0f, 0.5f, 0.5f)},
            {"Avatar", new Color(0.8f, 0.5f, 0.0f)},
            {"Booths", new Color(0.2f, 01f, 0.7f)},
            {"EventTicket", new Color(0.5f, 0.0f, 0.7f)},
            {"Material", new Color(1f, 0.6f, 0.8f)},
            {"Mesh", new Color(0.0f, 0.9f, 0.5f)},
            {"Particles", new Color(0.9f, 0.2f, 0.5f)},
            {"Settings", new Color(0.6f, 0.6f, 1f)},
            {"Text", new Color(0.3f, 0.5f, 0.9f)},
            {"Texture", new Color(0.9f, 0.2f, 0.2f)},
        };

        public static Texture2D GetRandomPaintSplat()
        {
            string[] splats = new string[]
            {
                "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat1.png",
                "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat2.png",
                "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat3.png",
                "Assets/PJKT SDK 2/Editor/Graphics/PaintSplat4.png"
            };
            
            int r = Random.Range(0, splats.Length);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(splats[r]);
        }
        
        public static Texture2D GetGraphic(string graphicName)
        {
            if (_graphicPaths.ContainsKey(graphicName))
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(_graphicPaths[graphicName]);
            }
            else
            {
                Debug.LogError("Graphic not found");
                return new Texture2D(1, 1);
            }
        }
        
        public static Color GetRandomColor()
        {
            int r = Random.Range(0, GraphicColors.Count -1);
            return GraphicColors.ElementAt(r).Value;
        }
    }
}