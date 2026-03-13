using System.Collections.Generic;
using PJKT.SDK.Prefabs;
using UnityEditor;
using UnityEngine;

namespace PJKT.SDK2
{
        [CustomEditor(typeof(PjktLanguageSwitcher))]
    public class PjktLanguageSwitcherEditor : Editor
    {
        private Dictionary<Language, string> languageTable = new Dictionary<Language, string>()
        {
            { Language.English,                      "en" },
            { Language.French,                       "fr" },
            { Language.Spanish,                      "es" },
            { Language.Italian,                      "it" },
            { Language.Korean,                       "ko" },
            { Language.German,                       "de" },
            { Language.Japanese,                     "ja" },
            { Language.Polish,                       "pl" },
            { Language.Russian,                      "ru" },
            { Language.BrazilianPortuguese,       "pt-BR" },
            { Language.ChineseSimplified,         "zh-CN" },
            { Language.ChineseTraditional,        "zh-HK" },
            { Language.Hebrew,                       "he" },
            { Language.TokiPona,                    "tok" },
            { Language.Ukrainian,                    "uk" },
            { Language.Thai,                         "th" }
        };
        
        private enum Language
        {
            English,
            French,
            Spanish,
            Italian,
            Korean,
            German,
            Japanese,
            Polish,
            Russian,
            BrazilianPortuguese,
            ChineseSimplified,
            ChineseTraditional,
            Hebrew,
            TokiPona,
            Ukrainian,
            Thai,
        }
        
        private SerializedProperty languagesProp;
        private SerializedProperty languageTextsProp;
        
        private void OnEnable()
        {
            languagesProp = serializedObject.FindProperty("languages");
            languageTextsProp = serializedObject.FindProperty("languageTexts");
        }

        public override void OnInspectorGUI()
        {
            PjktCommonGui.DrawComponentHeader("Language Switcher",
                "Automatically changes text based on the user's language settings.");

            serializedObject.Update();
            if (languagesProp.arraySize == 0)
            {
                AddNewLanguageOverride();
                serializedObject.ApplyModifiedProperties();
            }
            
            //draw the first one by itself so theres always at least one
            DrawLanguageOption(0);

            //draw any others
            for (int i = 1; i < languagesProp.arraySize; i++)
            {
                GUILayout.Space(5);
                DrawLanguageOption(i);
            }
            
            //button for adding more options
            GUILayout.Space(10);
            if (GUILayout.Button("Add Language Override"))
            {
                AddNewLanguageOverride();
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void AddNewLanguageOverride()
        {
            languagesProp.arraySize++;
            languageTextsProp.arraySize++;
            
            //default to english
            languagesProp.GetArrayElementAtIndex(languagesProp.arraySize - 1).stringValue = "en";
            languageTextsProp.GetArrayElementAtIndex(languageTextsProp.arraySize - 1).stringValue = "Enter Text Here";
        }

        private void DrawLanguageOption(int index)
        {
            GUI.backgroundColor = Color.gray;
            
            //figure out which enum corrisponds to the language
            string languageCode = languagesProp.GetArrayElementAtIndex(index).stringValue;
            Language languageEnum = Language.English;
            foreach (var kvp in languageTable)
            {
                if (kvp.Value != languageCode) continue;
                languageEnum = kvp.Key;
                break;
            }
            
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Language", GUILayout.Width(70));
                languageEnum = (Language)EditorGUILayout.EnumPopup(languageEnum);
                languagesProp.GetArrayElementAtIndex(index).stringValue = languageTable[languageEnum];
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(3);
                
                languageTextsProp.GetArrayElementAtIndex(index).stringValue =
                    EditorGUILayout.TextArea(languageTextsProp.GetArrayElementAtIndex(index).stringValue, GUILayout.MinHeight(60), GUILayout.ExpandHeight(true));
                
                //remove button
                GUI.backgroundColor = Color.red;
                GUILayout.Space(3);
                if (GUILayout.Button("Remove"))
                {
                    languagesProp.DeleteArrayElementAtIndex(index);
                    languageTextsProp.DeleteArrayElementAtIndex(index);
                }
                GUILayout.Space(3);
            }
            
            GUI.backgroundColor = Color.white;
        }
    }    
}