using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Volume))]
    public class VolumeObject : MonoBehaviour
    {
        public static List<VolumeObject> volumes;

        private void Awake()
        {
            if (volumes == null)
            {
                volumes = new();
            }

            volumes.Add(this);
        }

        private void Start()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
            GetComponent<Volume>().enabled = BetterPrefs.GetBool("PostProcessing", true);

            if (!BetterPrefs.GetBool("PostProcessing", true))
            {
                return;
            }

            Volume volume = GetComponent<Volume>();
            Bloom bloom;
            volume.profile.TryGet(out bloom);
            switch (BetterPrefs.GetInt("BloomQuality", 2))
            {
                case 0:
                    bloom.active = false;
                    bloom.highQualityFiltering = new(false, true);
                    break;
                case 1:
                    bloom.active = true;
                    bloom.highQualityFiltering = new(false, true);
                    bloom.maxIterations = new(2, 2, 8, true);
                    break;
                case 2:
                    bloom.highQualityFiltering = new(false, true);
                    bloom.maxIterations = new(4, 2, 8, true);
                    bloom.active = true;
                    break;
                case 3:
                    bloom.maxIterations = new(6, 2, 8, true);
                    bloom.highQualityFiltering = new(false, true);
                    bloom.active = true;
                    break;
                case 4:
                    bloom.maxIterations = new(8, 2, 8, true);
                    bloom.highQualityFiltering = new(true, true);
                    bloom.active = true;
                    break;
            }
        }

        private void OnDestroy()
        {
            volumes.Remove(this);
        }
    }
}