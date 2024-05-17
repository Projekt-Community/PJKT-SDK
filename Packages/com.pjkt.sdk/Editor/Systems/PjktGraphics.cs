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
            {"PjktLogo", "Packages/com.pjkt.sdk/Editor/Graphics/pjktLogo.png"},
            {"FestLogo", "Packages/com.pjkt.sdk/Editor/Graphics/PJKTFESTLogo.png"},
            {"Animation", "Packages/com.pjkt.sdk/Editor/Graphics/Animation.png"},
            {"Booths", "Packages/com.pjkt.sdk/Editor/Graphics/Booths.png"},
            {"EventTicket", "Packages/com.pjkt.sdk/Editor/Graphics/EventTicket.png"},
            {"Material", "Packages/com.pjkt.sdk/Editor/Graphics/Material.png"},
            {"Mesh", "Packages/com.pjkt.sdk/Editor/Graphics/Mesh.png"},
            {"PaintSplat1", "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat1.png"},
            {"PaintSplat2", "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat2.png"},
            {"PaintSplat3", "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat3.png"},
            {"PaintSplat4", "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat4.png"},
            {"paintLeft", "Packages/com.pjkt.sdk/Editor/Graphics/paintLeft.png"},
            {"paintRight", "Packages/com.pjkt.sdk/Editor/Graphics/paintRight.png"},
            {"Particles", "Packages/com.pjkt.sdk/Editor/Graphics/Particles.png"},
            {"Settings", "Packages/com.pjkt.sdk/Editor/Graphics/Settings.png"},
            {"Text", "Packages/com.pjkt.sdk/Editor/Graphics/Text.png"},
            {"Texture", "Packages/com.pjkt.sdk/Editor/Graphics/Texture.png"},
            {"Avatar", "Packages/com.pjkt.sdk/Editor/Graphics/AvatarPedistalIcon.png"},
            {"UdonBehaviour", "Packages/com.pjkt.sdk/Editor/Graphics/UdonBehaviourIcon.png"},
            {"Portal", "Packages/com.pjkt.sdk/Editor/Graphics/PortalIcon.png"},
            {"Pickup", "Packages/com.pjkt.sdk/Editor/Graphics/PickupIcon.png"},
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
                "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat1.png",
                "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat2.png",
                "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat3.png",
                "Packages/com.pjkt.sdk/Editor/Graphics/PaintSplat4.png"
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