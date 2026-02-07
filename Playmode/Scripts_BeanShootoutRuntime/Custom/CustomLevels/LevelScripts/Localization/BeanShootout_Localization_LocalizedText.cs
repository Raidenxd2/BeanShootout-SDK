using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KillItMyself.Runtime
{
    [AddComponentMenu("Bean Shootout/Localized Text")]
    public class BeanShootout_Localization_LocalizedText : MonoBehaviour
    {
        [Tooltip("UnityLegacyText: Select if your using the legacy text built-in to Unity.\nTextMeshProText: Select if your using TextMeshPro - Text\nTextMeshProTextUI: Select if your using TextMeshPro - Text (UI)")]
        public BeanShootout_Localization_TextType TextType;

        [Tooltip("The name of a key in the LocalizationKeys object")]
        public string KeyName;

        private void Start()
        {
            switch (TextType)
            {
                case BeanShootout_Localization_TextType.UnityLegacyText:
                    Text legacyText = GetComponent<Text>();

                    for (int i = 0; i < BeanShootout_Localization_LocalizationKeys.instance.keys.Count; i++)
                    {
                        if (BeanShootout_Localization_LocalizationKeys.instance.keys[i].Key == KeyName)
                        {
                            UpdateText(PlayerPrefs.GetString("selected-locale", "en"), legacyText, null, i);
                        }
                    }

                    break;
                case BeanShootout_Localization_TextType.TextMeshProText:
                    TMP_Text TextMeshProText = GetComponent<TMP_Text>();

                    for (int i = 0; i < BeanShootout_Localization_LocalizationKeys.instance.keys.Count; i++)
                    {
                        if (BeanShootout_Localization_LocalizationKeys.instance.keys[i].Key == KeyName)
                        {
                            UpdateText(PlayerPrefs.GetString("selected-locale", "en"), null, TextMeshProText, i);
                        }
                    }

                    break;
                case BeanShootout_Localization_TextType.TextMeshProTextUI:
                    TMP_Text TextMeshProTextUI = GetComponent<TMP_Text>();

                    for (int i = 0; i < BeanShootout_Localization_LocalizationKeys.instance.keys.Count; i++)
                    {
                        if (BeanShootout_Localization_LocalizationKeys.instance.keys[i].Key == KeyName)
                        {
                            UpdateText(PlayerPrefs.GetString("selected-locale", "en"), null, TextMeshProTextUI, i);
                        }
                    }

                    break;
            }
        }

        private void UpdateText(string lang, Text legacyText, TMP_Text TextMeshProText, int KeyIndex)
        {
            if (legacyText)
            {
                switch (lang)
                {
                    case "en":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_English;
                        break;
                    case "es":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Spanish;
                        break;
                    case "fr":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_French;
                        break;
                    case "ja":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Japanese;
                        break;
                    case "ko":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Korean;
                        break;
                    case "zh-CN":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_SimplifiedChinese;
                        break;
                    case "ru":
                        legacyText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Russian;
                        break;
                }
            }
            else
            {
                switch (lang)
                {
                    case "en":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_English;
                        break;
                    case "es":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Spanish;
                        break;
                    case "fr":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_French;
                        break;
                    case "ja":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Japanese;
                        break;
                    case "ko":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Korean;
                        break;
                    case "zh-CN":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_SimplifiedChinese;
                        break;
                    case "ru":
                        TextMeshProText.text = BeanShootout_Localization_LocalizationKeys.instance.keys[KeyIndex].Text_Russian;
                        break;
                }
            }
        }
    }

    public enum BeanShootout_Localization_TextType
    {
        UnityLegacyText = 0,
        TextMeshProText = 1,
        TextMeshProTextUI = 2
    }

#if UNITY_EDITOR
    public class LocalizationDev : Editor
    {
        [MenuItem("Bean Shootout/Dev/Localization/Set language to en")]
        public static void LanguageEn()
        {
            PlayerPrefs.SetString("selected-locale", "en");
        }

        [MenuItem("Bean Shootout/Dev/Localization/Set language to es")]
        public static void LanguageEs()
        {
            PlayerPrefs.SetString("selected-locale", "es");
        }

        [MenuItem("Bean Shootout/Dev/Localization/Set language to ja")]
        public static void LanguageJa()
        {
            PlayerPrefs.SetString("selected-locale", "ja");
        }
    }
#endif
}