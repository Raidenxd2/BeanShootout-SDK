using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class BulletGlobalOnline : NetworkBehaviour
    {
        public static BulletGlobalOnline instance;

        public NetworkVariable<int> Bullets = new();
        public NetworkVariable<bool> Reloading = new();

        [SerializeField] private TMP_Text BulletsText;

        [SerializeField] private GameObject Bullet;
        [SerializeField] private GameObject BulletParticle;

        private void Awake()
        {
            instance = this;

            if (!IsServer)
            {
                return;
            }

            Bullets.Value = GameSettings.MaxAmmo;
        }

        private void Update()
        {
            BulletsText.text = Bullets.Value.ToString();

            if (!IsServer)
            {
                return;
            }

            if (Bullets.Value <= 0 && !Reloading.Value)
            {
                BulletReloadRpc();
            }
        }

        [Rpc(SendTo.Server)]
        private void BulletReloadRpc()
        {
            StartCoroutine(BulletReload());
            Reloading.Value = true;
        }

        [Rpc(SendTo.Server)]
        public void ReduceBulletCountRpc(int val)
        {
            if (!IsServer)
            {
                return;
            }

            Bullets.Value -= val;
        }

        [Rpc(SendTo.Server)]
        public void SpawnBulletRpc(Vector3 Pos, Quaternion Rot, int Damage, bool ShootBackwards)
        {
            if (!IsServer)
            {
                return;
            }

            GameObject bulletGO = Instantiate(Bullet, Pos, Rot);

            bulletGO.GetComponent<NetworkObject>().Spawn();

            bulletGO.GetComponent<BulletMove>().ShootBackwards = ShootBackwards;
            bulletGO.GetComponent<BulletMove>().damage = Damage;

            Instantiate(BulletParticle, Pos, Rot);
        }

        private IEnumerator BulletReload()
        {
            yield return new WaitForSeconds(5f);
            Bullets.Value = GameSettings.MaxAmmo;
            Reloading.Value = false;
        }

        private new void OnDestroy()
        {
            Bullets.Dispose();
            Reloading.Dispose();

            instance = null;
        }
    }
}