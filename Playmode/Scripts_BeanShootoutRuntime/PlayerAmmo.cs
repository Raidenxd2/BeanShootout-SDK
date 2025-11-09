using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class PlayerAmmo : MonoBehaviour
    {
        public int ammo;
        private int oldBullets;
        public bool Reloading;

        [SerializeField] private TMP_Text AmmoText;
        [SerializeField] private GameObject AmmoRoot;

        private void Start()
        {
            ammo = GameSettings.MaxAmmo;

            if (GameSettings.SharedAmmo)
            {
                AmmoRoot.SetActive(false);
            }
            else
            {
                AmmoRoot.SetActive(true);
            }
        }

        private void FixedUpdate()
        {
            if (GameSettings.SharedAmmo)
            {
                return;
            }

            // Updates the BulletsText if Bullets updates
            if (ammo != oldBullets)
            {
                oldBullets = ammo;
                AmmoText.text = ammo.ToString();
            }

            // If Bullets is less or equal to 0 and we are not reloading, reload
            if (ammo <= 0 && !Reloading)
            {
                BulletReload().Forget();
                Reloading = true;
            }
        }

        private async UniTaskVoid BulletReload()
        {
            await UniTask.WaitForSeconds(5);
            ammo = GameSettings.MaxAmmo;
            Reloading = false;
        }
    }
}