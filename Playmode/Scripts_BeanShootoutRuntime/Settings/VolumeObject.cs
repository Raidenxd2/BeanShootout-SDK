using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Volume))]
    public class VolumeObject : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(BetterPrefs.GetBool("PostProcessing", true));

            if (BetterPrefs.GetBool("Bloom", true) == false && BetterPrefs.GetBool("PostProcessing", true))
            {
                Volume volume = GetComponent<Volume>();
                Bloom bloom;
                volume.profile.TryGet(out bloom);
                bloom.active = false;
            }
        }
    }
}