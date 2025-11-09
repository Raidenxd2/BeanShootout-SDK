using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KillItMyself.Runtime
{
    public class BulletManager : MonoBehaviour
    {
        [SerializeField] private GameObject BulletPrefab;
        [SerializeField] private Transform BulletParent;
        [SerializeField] private GameObject GunShootParticle;

        [SerializeField] private Transform Player1Transform;
        [SerializeField] private Transform BulletOffset;
        [SerializeField] private Transform BulletOffsetBehind;
        [SerializeField] private Transform GunShootParticleOffset;

        [SerializeField] private PlayerInput playerControls;

        public GunSO gun;
        [SerializeField] private Image gunVisual;

        [SerializeField] private Recoil recoil;

        [SerializeField] private PlayerAmmo playerAmmo;

        public bool CanShoot;
        public bool CannotShootNoMatterWhat;

        private void Update()
        {
            if (!gun)
            {
                return;
            }

            if (playerControls.actions["Shoot"].WasPressedThisFrame() && !gun.HoldToShoot)
            {
                Shoot();
            }
            else if (playerControls.actions["Shoot"].IsPressed() && gun.HoldToShoot)
            {
                Shoot();
            }
        }

        public void BulletManagerInit()
        {
            gunVisual.sprite = gun.Image;
        }

        private void Shoot()
        {
            if (PauseManager.instance.paused)
            {
                return;
            }

            // If were in an online game, check the online BulletGlobal if were reloading, else check the local BulletGlobal
            if (OnlineManager.instance.InOnlineGame && GameSettings.SharedAmmo)
            {
                if (BulletGlobalOnline.instance.Reloading.Value || !CanShoot)
                {
                    return;
                }
            }
            else if (!GameSettings.SharedAmmo)
            {
                if (playerAmmo.Reloading || !CanShoot)
                {
                    return;
                }
            }
            else if (BulletGlobal.instance.Reloading || !CanShoot)
            {
                return;
            }

            if (CannotShootNoMatterWhat)
            {
                return;
            }

            // If were in an online game, Reduce the amount of bullets for the online BulletGlobal, else reduce the amont of bullets for the local BulletGlobal
            if (OnlineManager.instance.InOnlineGame && GameSettings.SharedAmmo)
            {
                BulletGlobalOnline.instance.ReduceBulletCountRpc(gun.BulletsThatAreUsed);
            }
            else if (GameSettings.SharedAmmo)
            {
                BulletGlobal.instance.Bullets -= gun.BulletsThatAreUsed;
            }
            else
            {
                playerAmmo.ammo -= gun.BulletsThatAreUsed;
            }

            if (gun.Delay > 0)
            {
                DelayShot().Forget();
            }
            
            if (OnlineManager.instance.InOnlineGame)
            {
                recoil.FireRecoilRpc();
            }
            else
            {
                recoil.FireRecoil();
            }
            
            Quaternion rot = Quaternion.Euler(new Vector3(Player1Transform.eulerAngles.x, Player1Transform.eulerAngles.y, Player1Transform.eulerAngles.z));

            if (OnlineManager.instance.InOnlineGame)
            {
                BulletGlobalOnline.instance.SpawnBulletParticleRpc(BulletOffset.position, rot);
            }
            else
            {
                Instantiate(GunShootParticle, GunShootParticleOffset.position, Quaternion.Euler(new Vector3(0, 0, 0)), BulletParent);
            }

            for (int i = 0; i < gun.AmountOfBulletsToShoot; i++)
            {
                BulletMove bullet = null;

                // Spawn bullet and if the gun shoots backwards, face the opossite direction of the players camera, else face the players camera
                if (gun.ShootBackwards)
                {
                    if (OnlineManager.instance.InOnlineGame)
                    {
                        BulletGlobalOnline.instance.SpawnBulletRpc(BulletOffsetBehind.position, rot, gun.Damage, gun.ShootBackwards, NetworkManager.Singleton.LocalClientId);
                    }
                    else
                    {
                        bullet = Instantiate(BulletPrefab, BulletOffsetBehind.position, rot, BulletParent).GetComponent<BulletMove>();
                    }
                }
                else
                {
                    if (OnlineManager.instance.InOnlineGame)
                    {
                        BulletGlobalOnline.instance.SpawnBulletRpc(BulletOffset.position, rot, gun.Damage, gun.ShootBackwards, NetworkManager.Singleton.LocalClientId);
                    }
                    else
                    {
                        bullet = Instantiate(BulletPrefab, BulletOffset.position, rot, BulletParent).GetComponent<BulletMove>();
                    }
                }
                
                // If were not in an online game, set the bullets damage and set shoot backwards
                if (!OnlineManager.instance.InOnlineGame)
                {
                    bullet.damage = gun.Damage;

                    if (gun.ShootBackwards)
                    {
                        bullet.ShootBackwards = true;
                    }
                }
            }
        }
        
        private async UniTask DelayShot()
        {
            CanShoot = false;
            await UniTask.WaitForSeconds(gun.Delay);
            CanShoot = true;
        }
    }
}