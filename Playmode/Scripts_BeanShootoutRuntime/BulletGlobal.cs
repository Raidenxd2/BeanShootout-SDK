using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace KillItMyself.Runtime
{
    /// <summary>
    /// BulletGlobal is only used if GameSettings.SharedAmmo is enabled
    /// </summary>
    public class BulletGlobal : MonoBehaviour
    {
        public static BulletGlobal instance;

        public int Bullets;
        private int oldBullets;
        public bool Reloading;

        [SerializeField] private TMP_Text BulletsText;
        [SerializeField] private GameObject BulletsRoot;

        private void Awake()
        {
            if (!GameSettings.SharedAmmo)
            {
                BulletsRoot.SetActive(false);
                return;
            }

            instance = this;

            Bullets = GameSettings.MaxAmmo;
        }

        private void Update()
        {
            if (!GameSettings.SharedAmmo)
            {
                return;
            }

            // Updates the BulletsText if Bullets updates
            if (Bullets != oldBullets)
            {
                oldBullets = Bullets;
                BulletsText.text = Bullets.ToString();
            }

            // If Bullets is less or equal to 0 and we are not reloading, reload
            if (Bullets <= 0 && !Reloading)
            {
                BulletReload().Forget();
                Reloading = true;
            }
        }

        private async UniTaskVoid BulletReload()
        {
            await UniTask.WaitForSeconds(5);
            Bullets = GameSettings.MaxAmmo;
            Reloading = false;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}