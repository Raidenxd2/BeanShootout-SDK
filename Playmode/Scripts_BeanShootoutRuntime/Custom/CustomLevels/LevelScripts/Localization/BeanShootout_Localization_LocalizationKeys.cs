using System;
using System.Collections.Generic;
using UnityEngine;

namespace KillItMyself.Runtime
{
    [AddComponentMenu("Bean Shootout/Localization Keys")]
    public class BeanShootout_Localization_LocalizationKeys : MonoBehaviour
    {
        public static BeanShootout_Localization_LocalizationKeys instance;

        public List<BeanShootout_Localization_LocalizationKey> keys = new();

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }

    [Serializable]
    public class BeanShootout_Localization_LocalizationKey
    {
        public string Key;
        public string Text_English;
        public string Text_Spanish;
        public string Text_French;
        public string Text_Japanese;
        public string Text_Korean;
        public string Text_SimplifiedChinese;
        public string Text_Russian;
    }
}