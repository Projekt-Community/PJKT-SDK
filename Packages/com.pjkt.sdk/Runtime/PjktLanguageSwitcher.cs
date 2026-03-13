using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PJKT.SDK.Prefabs
{
    [UdonBehaviourSyncMode(behaviourSyncMode: BehaviourSyncMode.None)]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PjktLanguageSwitcher : UdonSharpBehaviour
    {
        [SerializeField] private string[] languages;
        [SerializeField] private string[] languageTexts;
        [SerializeField] TextMeshProUGUI textComp;

        private void Start()
        {
            if (languages.Length == 0) return;
            string currentLanguage = VRCPlayerApi.GetCurrentLanguage();
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i] != currentLanguage) continue;
                textComp.text = languageTexts[i];
                break;
            }
        }

        public override void OnLanguageChanged(string language)
        {
            if (languages.Length == 0) return;
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i] != language) continue;
                textComp.text = languageTexts[i];
                break;
            }
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        private void Reset()
        {
            textComp = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}