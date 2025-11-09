using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class ShotgunBlast : MonoBehaviour
    {
        [SerializeField] private ParticleSystem VFX;
        private void OnEnable()
        {
#pragma warning disable CS0618
            float totalDuration = VFX.main.duration + VFX.startLifetime;
#pragma warning restore CS0618
            if (OnlineManager.instance.InOnlineGame && GetComponent<NetworkObject>().IsOwner)
            {
                StartCoroutine(DespawnWait(totalDuration));
            }
            else
            {
                Destroy(gameObject, totalDuration);
            }
        }

        private IEnumerator DespawnWait(float totalDuration)
        {
            yield return new WaitForSeconds(totalDuration);

            if (gameObject)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}