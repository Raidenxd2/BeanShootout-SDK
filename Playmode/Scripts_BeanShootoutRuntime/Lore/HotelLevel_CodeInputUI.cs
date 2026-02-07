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
    public class HotelLevel_CodeInputUI : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerControls;

        [SerializeField] private GameObject XboxControllerIcons;
        [SerializeField] private GameObject UniversalControllerIcons;
        [SerializeField] private Transform ControllerIconsParent;

        [SerializeField] private TMP_InputField CodeInput;

        [SerializeField] private PlayerMovement playerMovement;

        private void Start()
        {
            if (playerControls.devices[0].displayName.Contains("Xbox"))
            {
                Instantiate(XboxControllerIcons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
        }

        public void InputButton()
        {
#if KILLITMYSELF_FULL
            if (CodeInput.text.Equals("destructionprotocol"))
            {
                playerMovement.HotelLevel_CodeInputInteractUI.SetActive(false);
                InputButtonAsync().Forget();
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

            await SceneManager.LoadSceneAsync(SceneNames.S_DroneDestroyBarricade, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneNames.S_DroneDestroyBarricade));

            await UniTask.WaitForEndOfFrame();
            CutsceneCamera.instance.playerControls = playerControls;
            CutsceneCamera.instance.InitCutsceneCamera();
            SavingRootObject.instance.LoadingAssetRoot.SetActive(false);
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