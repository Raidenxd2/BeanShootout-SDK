using Cysharp.Threading.Tasks;
#if KILLITMYSELF_FULL
using KillItMyself.Lore.PartOne;
using KillItMyself.Runtime.Achievements;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace KillItMyself.Runtime
{
    public class ShipLevel_OverrideCodeUI : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerControls;

        [SerializeField] private GameObject XboxControllerIcons;
        [SerializeField] private GameObject UniversalControllerIcons;
        [SerializeField] private Transform ControllerIconsParent;

        [SerializeField] private TMP_Text OneText;
        [SerializeField] private TMP_Text TwoText;
        [SerializeField] private TMP_Text ThreeText;
        [SerializeField] private TMP_Text FourText;
        [SerializeField] private TMP_Text FiveText;

        [SerializeField] private TMP_Text CodeText;

        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private BulletManager bulletManager;
        [SerializeField] private PlayerCam playerCam;

#if KILLITMYSELF_FULL
        [SerializeField] private AchievementSO UnderTheSurfaceAchievement;
#endif

        private int One;
        private int Two;
        private int Three;
        private int Four;
        private int Five;

        private void Start()
        {
#if KILLITMYSELF_FULL
            // if (LorePartOneVariables.OverrideCodeFound)
            // {
                CodeText.text = LorePartOneVariables.OverrideCode.ToString();
            // }
#endif

            if (playerControls.devices[0].displayName.Contains("Xbox"))
            {
                Instantiate(XboxControllerIcons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
        }

        public void OneUp()
        {
#if KILLITMYSELF_FULL
            One++;
            if (One >= 10)
            {
                One = 0;
            }

            OneText.text = One.ToString();
#endif
        }

        public void TwoUp()
        {
#if KILLITMYSELF_FULL
            Two++;
            if (Two >= 10)
            {
                Two = 0;
            }

            TwoText.text = Two.ToString();
#endif
        }

        public void ThreeUp()
        {
#if KILLITMYSELF_FULL
            Three++;
            if (Three >= 10)
            {
                Three = 0;
            }

            ThreeText.text = Three.ToString();
#endif
        }

        public void FourUp()
        {
#if KILLITMYSELF_FULL
            Four++;
            if (Four >= 10)
            {
                Four = 0;
            }

            FourText.text = Four.ToString();
#endif
        }

        public void FiveUp()
        {
#if KILLITMYSELF_FULL
            Five++;
            if (Five >= 10)
            {
                Five = 0;
            }

            FiveText.text = Five.ToString();
#endif
        }

        public void InputButton()
        {
#if KILLITMYSELF_FULL
            string inputCode = LorePartOneVariables.OverrideCode.ToString();
            string code = One.ToString() + Two.ToString() + Three.ToString() + Four.ToString() + Five.ToString();
            if (code == inputCode)
            {
                InputButtonAsync().Forget();
            }
#endif
        }

#if KILLITMYSELF_FULL
        private async UniTaskVoid InputButtonAsync()
        {
            AchievementManager.instance.GrantAchievement(UnderTheSurfaceAchievement);
            SaveManager.instance.SaveCurrent();

            gameObject.SetActive(false);
            playerMovement.canMove = false;
            bulletManager.CannotShootNoMatterWhat = true;
            playerCam.canMoveCamera = false;

            Cursor.lockState = CursorLockMode.Locked;

            await LoadingManager.instance.ShowLoadingScreen(true);

            SavingRootObject.instance.LoadingAssetRoot.SetActive(true);
            await UniTask.WaitForEndOfFrame();

            await SceneManager.LoadSceneAsync(AddressableReferences.S_EnterCodeCutsceneShip, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(AddressableReferences.S_EnterCodeCutsceneShip));

            await UniTask.WaitForEndOfFrame();
            CutsceneCamera.instance.playerControls = playerControls;
            CutsceneCamera.instance.InitCutsceneCamera();
            SavingRootObject.instance.LoadingAssetRoot.SetActive(false);
            GameObject.Find("EnterCodeCutscene-Ship-Root").GetComponent<EnterCodeCutscene_Ship>().playerControls = playerControls;
            await LoadingManager.instance.HideLoadingScreen(true);
        }
#endif

        private void Update()
        {
            if (playerMovement.IsOnKeyboardMouse)
            {
                Cursor.lockState = CursorLockMode.None;
            }


        }
    }
}