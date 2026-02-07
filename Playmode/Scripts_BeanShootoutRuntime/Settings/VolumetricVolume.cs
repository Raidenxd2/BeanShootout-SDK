using UnityEngine;
using UnityEngine.Rendering;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Volume))]
    public class VolumetricVolume : MonoBehaviour
    {
        [SerializeField] private Volume volume;

        public static VolumetricVolume instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void Start()
        {
            UpdateValues();
        }

        public void UpdateValues()
        {
#if KILLITMYSELF_FULL
            switch (BetterPrefs.GetInt("VolumetricLightingQuality", 0))
            {
                case 0:
                    volume.profile.TryGet(out VolumetricFogVolumeComponent volumetricComponent);

                    volumetricComponent.additionalLightsContribution = new(false, true);
                    volumetricComponent.enabled = new(false, true);
                    volumetricComponent.active = false;
                    break;
                case 1:
                    volume.profile.TryGet(out VolumetricFogVolumeComponent volumetricComponent2);

                    volumetricComponent2.additionalLightsContribution = new(true, true);
                    volumetricComponent2.enabled = new(true, true);
                    volumetricComponent2.active = true;

                    volumetricComponent2.resolution = new(VolumetricFogResolution.Quarter, true);

                    volumetricComponent2.maximumSteps = new(4, 4, 128, true);
                    volumetricComponent2.blurIterations = new(1, 1, 6, true);
                    break;
                case 2:
                    volume.profile.TryGet(out VolumetricFogVolumeComponent volumetricComponent3);

                    volumetricComponent3.additionalLightsContribution = new(true, true);
                    volumetricComponent3.enabled = new(true, true);
                    volumetricComponent3.active = true;

                    volumetricComponent3.resolution = new(VolumetricFogResolution.Quarter, true);

                    volumetricComponent3.maximumSteps = new(32, 4, 128, true);
                    volumetricComponent3.blurIterations = new(2, 1, 6, true);
                    break;
                case 3:
                    volume.profile.TryGet(out VolumetricFogVolumeComponent volumetricComponent4);

                    volumetricComponent4.additionalLightsContribution = new(true, true);
                    volumetricComponent4.enabled = new(true, true);
                    volumetricComponent4.active = true;

                    volumetricComponent4.resolution = new(VolumetricFogResolution.Quarter, true);

                    volumetricComponent4.maximumSteps = new(64, 4, 128, true);
                    volumetricComponent4.blurIterations = new(4, 1, 6, true);
                    break;
                case 4:
                    volume.profile.TryGet(out VolumetricFogVolumeComponent volumetricComponent5);

                    volumetricComponent5.additionalLightsContribution = new(true, true);
                    volumetricComponent5.enabled = new(true, true);
                    volumetricComponent5.active = true;

                    volumetricComponent5.resolution = new(VolumetricFogResolution.Half, true);

                    volumetricComponent5.maximumSteps = new(128, 4, 128, true);
                    volumetricComponent5.blurIterations = new(6, 1, 6, true);
                    break;
                default:
                    break;
            }
#endif
        }
    }
}