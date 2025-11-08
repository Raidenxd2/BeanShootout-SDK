#if KILLITMYSELF_FULL
using KillItMyself.Runtime.Content.Resources;
#endif

using UnityEngine;

namespace KillItMyself.Runtime
{
    [AddComponentMenu("Bean Shootout/Audio Type")]
    [RequireComponent(typeof(AudioSource))]
    public class AudioSource_AudioType : MonoBehaviour
    {
        [Tooltip("The setting you set here allows you to change the volume in the game's audio settings")]
        [SerializeField] private CustomLevelAudioType AudioType;

#if KILLITMYSELF_FULL
        private void Start()
        {
            switch (AudioType)
            {
                case CustomLevelAudioType.Other:
                    GetComponent<AudioSource>().outputAudioMixerGroup = ResourcesReferences.instance.GameAudioMixer.FindMatchingGroups("Master")[0];
                    break;
                case CustomLevelAudioType.Music:
                    GetComponent<AudioSource>().outputAudioMixerGroup = ResourcesReferences.instance.GameAudioMixer.FindMatchingGroups("Music")[0];
                    break;
                case CustomLevelAudioType.Sound:
                    GetComponent<AudioSource>().outputAudioMixerGroup = ResourcesReferences.instance.GameAudioMixer.FindMatchingGroups("SFX")[0];
                    break;
                case CustomLevelAudioType.Ambience:
                    GetComponent<AudioSource>().outputAudioMixerGroup = ResourcesReferences.instance.GameAudioMixer.FindMatchingGroups("Ambience")[0];
                    break;
            }
        }
#endif
    }

    public enum CustomLevelAudioType
    {
        Other,
        Music,
        Sound,
        Ambience
    }
}