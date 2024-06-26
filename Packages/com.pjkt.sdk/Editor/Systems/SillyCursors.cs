using UnityEditor;
using UnityEngine;
using Cursor = UnityEngine.UIElements.Cursor;

namespace PJKT.SDK2.Extras
{
    public static class SillyCursors
    {
        private static readonly string[] cursorLocations = new string[]
        {
            "Packages/com.pjkt.sdk/Editor/Graphics/scimmy.png",
            "Packages/com.pjkt.sdk/Editor/Graphics/lil Guantlet.png"
        };
        public static Cursor GetSillyCursor()
        {
            Cursor cursor = new Cursor();
            cursor.hotspot = new Vector2(0, 0);

            int r = Random.Range(0, cursorLocations.Length);
            cursor.texture = AssetDatabase.LoadAssetAtPath<Texture2D>(cursorLocations[r]);
            return cursor;
        }
    }   
}