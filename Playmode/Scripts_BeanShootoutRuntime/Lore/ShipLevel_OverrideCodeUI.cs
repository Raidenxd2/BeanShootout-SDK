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

        private int One;
        private int Two;
        private int Three;
        private int Four;
        private int Five;

        [SerializeField] private PlayerMovement playerMovement;

#if KILLITMYSELF_FULL
        [SerializeField] private AchievementSO UnderTheSurfaceAchievement;
        [SerializeField] private AchievementSO BilingualAchievement;
#endif

        private void Start()
        {
#if KILLITMYSELF_FULL
            if (LorePartOneVariables.OverrideCodeFound && SceneManager.GetActiveScene().name == SceneNames.S_ShipLevel)
            {
                CodeText.text = LorePartOneVariables.OverrideCode.ToString();
            }
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
            if (SceneManager.GetActiveScene().name == SceneNames.S_SkyHotelLevel)
            {
                string inputCode = 57385.ToString();
                string code = One.ToString() + Two + Three + Four + Five;
                if (code == inputCode)
                {
                    gameObject.SetActive(false);
                    
                    playerMovement.ShipLevel_OverrideCodeInteractUI.SetActive(false);
                    AchievementManager.instance.GrantAchievement(UnderTheSurfaceAchievement);

                    GameObject.Find("HotelLevel_Elevator").GetComponent<HotelLevel_Elevator>().UnlockElevator();
                    
                    playerMovement.LetPlayerDoAnything();
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else if (SceneManager.GetActiveScene().name == SceneNames.S_MoonbaseBetaLevel)
            {
                string inputCode = 16790.ToString();
                string code = One.ToString() + Two + Three + Four + Five;
                if (code == inputCode)
                {
                    gameObject.SetActive(false);
                    playerMovement.ShipLevel_OverrideCodeInteractUI.SetActive(false);
                    AchievementManager.instance.GrantAchievement(BilingualAchievement);
                    
                    playerMovement.LetPlayerDoAnything();
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
            else
            {
                string inputCode = LorePartOneVariables.OverrideCode.ToString();
                string code = One.ToString() + Two + Three + Four + Five;
                if (code == inputCode)
                {
                    playerMovement.ShipLevel_OverrideCodeInteractUI.SetActive(false);
                    InputButtonAsync().Forget();
                }

                if (code != inputCode)
                {
                    BeanLogger.Log("Exploding ship cause no", this);
                }
            }
#endif
        }

#if KILLITMYSELF_FULL
        private async UniTaskVoid InputButtonAsync()
        {
            gameObject.SetActive(false);
            for (int i = 0; i < PlayersJoined.instance.Players.Count; i++)
            {
                PlayersJoined.instance.Players[i].GetComponent<PlayerMovement>().PreventPlayerFromDoingAnything();
            }

            Cursor.lockState = CursorLockMode.Locked;

            await LoadingManager.instance.ShowLoadingScreen(true);

            SavingRootObject.instance.LoadingAssetRoot.SetActive(true);
            await UniTask.WaitForEndOfFrame();

            await SceneManager.LoadSceneAsync(SceneNames.S_EnterCodeCutsceneShip, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneNames.S_EnterCodeCutsceneShip));

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