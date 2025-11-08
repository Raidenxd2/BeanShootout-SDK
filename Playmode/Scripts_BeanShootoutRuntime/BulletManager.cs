using Cysharp.Threading.Tasks;
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

        public bool CanShoot;
        public bool CannotShootNoMatterWhat;

        private void Update()
        {
            if (gun == null)
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
            if (OnlineManager.instance.InOnlineGame)
            {
                if (BulletGlobalOnline.instance.Reloading.Value || !CanShoot)
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
            if (OnlineManager.instance.InOnlineGame)
            {
                BulletGlobalOnline.instance.ReduceBulletCountRpc(gun.BulletsThatAreUsed);
            }
            else
            {
                BulletGlobal.instance.Bullets -= gun.BulletsThatAreUsed;
            }

            if (gun.Delay > 0)
            {
                DelayShot().Forget();
            }

            for (int i = 0; i < gun.AmountOfBulletsToShoot; i++)
            {
                if (OnlineManager.instance.InOnlineGame)
                {
                    recoil.FireRecoilRpc();
                }
                else
                {
                    recoil.FireRecoil();
                }

                GameObject bullet = null;

                // Spawn bullet and if the gun shoots backwards, face the opossite direction of the players camera, else face the players camera
                if (gun.ShootBackwards)
                {
                    if (OnlineManager.instance.InOnlineGame)
                    {
                        BulletGlobalOnline.instance.SpawnBulletRpc(BulletOffsetBehind.position, Quaternion.Euler(new Vector3(Player1Transform.eulerAngles.x, Player1Transform.eulerAngles.y, Player1Transform.eulerAngles.z)), gun.Damage, gun.ShootBackwards);
                    }
                    else
                    {
                        bullet = Instantiate(BulletPrefab, BulletOffsetBehind.position, Quaternion.Euler(new Vector3(Player1Transform.eulerAngles.x, Player1Transform.eulerAngles.y, Player1Transform.eulerAngles.z)), BulletParent);
                        Instantiate(GunShootParticle, GunShootParticleOffset.position, Quaternion.Euler(new Vector3(0, 0, 0)), BulletParent);
                    }
                }
                else
                {
                    if (OnlineManager.instance.InOnlineGame)
                    {
                        BulletGlobalOnline.instance.SpawnBulletRpc(BulletOffset.position, Quaternion.Euler(new Vector3(Player1Transform.eulerAngles.x, Player1Transform.eulerAngles.y, Player1Transform.eulerAngles.z)), gun.Damage, gun.ShootBackwards);
                    }
                    else
                    {
                        bullet = Instantiate(BulletPrefab, BulletOffset.position, Quaternion.Euler(new Vector3(Player1Transform.eulerAngles.x, Player1Transform.eulerAngles.y, Player1Transform.eulerAngles.z)), BulletParent);
                        Instantiate(GunShootParticle, GunShootParticleOffset.position, Quaternion.Euler(new Vector3(0, 0, 0)), BulletParent);
                    }
                }
                
                // If were not in an online game, set the bullets damage and set shoot backwards
                if (!OnlineManager.instance.InOnlineGame)
                {
                    bullet.GetComponent<BulletMove>().damage = gun.Damage;

                    if (gun.ShootBackwards)
                    {
                        bullet.GetComponent<BulletMove>().ShootBackwards = true;
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