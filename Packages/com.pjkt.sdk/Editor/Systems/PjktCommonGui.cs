using UnityEditor;
using UnityEngine;

namespace PJKT.SDK2
{
    /// <summary>
    /// basically just functions for drawing comon ui stuff
    /// ment to reduce copy pasting of code
    /// </summary>
    public static class PjktCommonGui
    {
        /// <summary>
        /// draws the component header
        /// </summary>
        private static Texture2D headerImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/PJKT/Pjkt Prefabs/Resources/header.png");
        private static Texture2D backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/PJKT/Pjkt Prefabs/Resources/background.png");
        
        public static void DrawComponentHeader(string title, string subtitle)
        {
            if (headerImage != null && backgroundImage != null)
            {
                Rect headerRect = EditorGUILayout.GetControlRect(false, 80, GUILayout.ExpandWidth(true));
                
                GUI.DrawTexture(headerRect, backgroundImage, ScaleMode.ScaleAndCrop);
                
                float leftMargin = 10f;
                float logoSize = headerRect.height;
                Rect logoRect = new Rect(headerRect.x + leftMargin, headerRect.y, logoSize, logoSize);
                
                GUI.DrawTexture(logoRect, headerImage, ScaleMode.ScaleToFit);
                
                GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                
                GUIStyle subtitleStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    fontSize = 14,
                    normal = { textColor = Color.white }
                };
                
                float textLeftMargin = logoSize + leftMargin + 10;
                Rect titleRect = new Rect(headerRect.x + textLeftMargin, headerRect.y + 15, 
                    headerRect.width - textLeftMargin, headerRect.height / 2 - 5);
                Rect subtitleRect = new Rect(headerRect.x + textLeftMargin, headerRect.y + headerRect.height / 2 - 5, 
                    headerRect.width - textLeftMargin, headerRect.height / 2);
                
                GUI.Label(titleRect, title, titleStyle);
                GUI.Label(subtitleRect, subtitle, subtitleStyle);
                
                EditorGUILayout.Space(5);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                var leftAlignedStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 20,
                    fontStyle = FontStyle.Bold
                };
                EditorGUILayout.LabelField(title, leftAlignedStyle);
                EditorGUILayout.LabelField(subtitle, new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleLeft, wordWrap = true, fontSize = 14 });
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }
        }
    }
}