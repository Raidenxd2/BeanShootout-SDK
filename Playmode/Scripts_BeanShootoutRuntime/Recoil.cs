using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class Recoil : NetworkBehaviour
    {
        [SerializeField] private RectTransform rt;

        [SerializeField] private float speed;
        [SerializeField] private Vector3 rot;
        [SerializeField] private Vector3 ReloadingRot;

        [SerializeField] private PlayerAmmo ammo;

        private void Update()
        {
            if (!GameSettings.SharedAmmo)
            {
                if (ammo.Reloading)
                {
                    rt.localRotation = Quaternion.Euler(rt.localRotation.eulerAngles + ReloadingRot);

                    return;
                }
            }
            else
            {
                if (OnlineManager.instance.InOnlineGame && BulletGlobalOnline.instance.Reloading.Value)
                {
                    rt.localRotation = Quaternion.Euler(rt.localRotation.eulerAngles + ReloadingRot);

                    return;
                }
                else if (BulletGlobal.instance.Reloading)
                {
                    rt.localRotation = Quaternion.Euler(rt.localRotation.eulerAngles + ReloadingRot);

                    return;
                }
            }
            // if (ammo.Reloading || (GameSettings.SharedAmmo && (BulletGlobal.instance.Reloading)) || (OnlineManager.instance.InOnlineGame && GameSettings.SharedAmmo && BulletGlobalOnline.instance.Reloading.Value))
            // {
                
            // }
            
            rt.localRotation = Quaternion.Slerp(rt.localRotation, Quaternion.Euler(0, -107.78f, 0), speed * Time.deltaTime);
        }

        public void FireRecoil()
        {
            rt.localRotation = Quaternion.Euler(rt.localRotation.eulerAngles + rot);
        }

        [Rpc(SendTo.Everyone)]
        public void FireRecoilRpc()
        {
            FireRecoil();
        }

        public void UpdateValuesForCurrentGun(GunSO gun)
        {
            speed = gun.RecoilSpeed;
            rot = gun.RecoilRot;
        }
    }
}