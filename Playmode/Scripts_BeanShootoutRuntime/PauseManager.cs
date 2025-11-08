#if KILLITMYSELF_FULL
using KillItMyself.Runtime.Content.Resources;
#endif
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class PauseManager : MonoBehaviour
    {
        public bool paused;

        [SerializeField] private GameObject PauseScreen;

        [SerializeField] private GameObject LoadingAsset;

        [SerializeField] private Transform SettingsMenuParent;

        private CursorLockMode prevCursorLock;
        private bool prevCanMove;

        public static PauseManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void PauseOrUnpause()
        {
            paused = !paused;

            if (paused)
            {
                SetTimeScale(0);

                prevCursorLock = Cursor.lockState;

                Cursor.lockState = CursorLockMode.None;

                PauseScreen.SetActive(true);
            }
            else
            {
                SetTimeScale(1);

                Cursor.lockState = prevCursorLock;

                PauseScreen.SetActive(false);
            }

            if (OnlineManager.instance.InOnlineGame)
            {
                if (paused)
                {
                    prevCanMove = CurrentPlayer.instance.playerMovement.canMove;
                    CurrentPlayer.instance.playerMovement.canMove = false;
                }
                else
                {
                    CurrentPlayer.instance.playerMovement.canMove = prevCanMove;
                }   
            }
        }

        public void ResumeGame()
        {
            paused = false;

            SetTimeScale(1);

            Cursor.lockState = prevCursorLock;

            PauseScreen.SetActive(false);
        }

#if KILLITMYSELF_FULL
        public void OpenSettingsMenu()
        {
            OpenSettingsMenuAsync();
        }

        public void CloseSettingsMenu()
        {
            SettingsMenuParent.gameObject.SetActive(false);

            Destroy(SettingsMenuParent.GetChild(0).gameObject);
        }

        private void OpenSettingsMenuAsync()
        {
            try
            {
                SettingsMenuParent.gameObject.SetActive(true);
                LoadingAsset.SetActive(true);

                GameObject settingsGO = Instantiate(ResourcesReferences.instance.SettingsPrefab, SettingsMenuParent);

                settingsGO.GetComponent<PauseMenuSettings>().pm = this;

                LoadingAsset.SetActive(false);
            }
            catch
            {
                DialogManager.instance.ShowDialog(DialogButtonType.OKButton);

                LoadingAsset.SetActive(false);
            }
        }
#endif

        private void SetTimeScale(float val)
        {
            if (!OnlineManager.instance.InOnlineGame)
            {
                Time.timeScale = val;
            }
        }

        public void QuitGame()
        {
            Time.timeScale = 1;

            PlayerCam.ChangePlayerHasJoined();

#if KILLITMYSELF_FULL
            if (OnlineManager.instance.InOnlineGame)
            {
                OnlineManager.instance.InOnlineGame = false;
            
                OnlineManager.instance.DisconnectAsHost();
                OnlineManager.instance.DisconnectAndLoadMainMenu();
                return;
            }

            LoadingManager.instance.LoadSceneWithoutLoadingScreen(AddressableReferences.S_MainMenu);
#else
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
#endif
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}