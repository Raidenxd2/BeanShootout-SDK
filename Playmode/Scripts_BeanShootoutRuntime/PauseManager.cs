#if KILLITMYSELF_FULL
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using com.raiden.assetbundleassetreference.Runtime;
#endif
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    public class PauseManager : MonoBehaviour
    {
        public bool paused;

        [SerializeField] private GameObject PauseScreen;

        [SerializeField] private GameObject LoadingAsset;

        [SerializeField] private GameObject BotPlayerPrefab;
        
#if UNITY_EDITOR        
        [SerializeField] private bool UseAssetBundlesInEditor;
#endif        
        
        [SerializeField] private Transform SettingsRootParent;
        private GameObject SettingsRoot;
#if KILLITMYSELF_FULL
        [SerializeField] private AssetBundleAssetReference SettingsRootRef;
        [SerializeField] private AssetBundleAssetReference SettingsRootSharedRef;
#endif
        private AssetBundle SettingsRootSharedBundle;
        private AssetBundle SettingsRootBundle;

        [SerializeField] private GameObject ResumeGameButton;
        [SerializeField] private GameObject NoButton;

        public CursorLockMode prevCursorLock;
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
                
                if (OnlineManager.instance.InOnlineGame)
                {
                    CurrentPlayer.instance.playerMovement.PreventPlayerFromDoingAnything();
                }

                prevCursorLock = Cursor.lockState;

                Cursor.lockState = CursorLockMode.None;

                PauseScreen.SetActive(true);

                EventSystem.current.SetSelectedGameObject(ResumeGameButton);
            }
            else
            {
                SetTimeScale(1);
                
                if (OnlineManager.instance.InOnlineGame)
                {
                    CurrentPlayer.instance.playerMovement.LetPlayerDoAnything();
                }

                Cursor.lockState = prevCursorLock;

                PauseScreen.SetActive(false);
            }
        }

        public void ShowExitGameScreen()
        {
            EventSystem.current.SetSelectedGameObject(NoButton);
        }

        public void DontShowExitGameScreen()
        {
            EventSystem.current.SetSelectedGameObject(ResumeGameButton);
        }

#if KILLITMYSELF_FULL
        public void OpenSettingsMenu()
        {
            ShowSettingsAsync().Forget();
        }

        private async UniTaskVoid ShowSettingsAsync()
        {
            SavingRootObject.instance.LoadingAssetRoot.SetActive(true);
            
#if UNITY_EDITOR
            if (!UseAssetBundlesInEditor)
            {
                SettingsRoot = Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/SettingsRoot Variant.prefab"), SettingsRootParent);
            }
            else
            {
#endif 
                SettingsRootSharedBundle = await AssetBundle.LoadFromFileAsync(Constants.GetAssetBundleFullPathPath(SettingsRootSharedRef.BundleName));
                SettingsRootBundle = await AssetBundle.LoadFromFileAsync(Constants.GetAssetBundleFullPathPath(SettingsRootRef.BundleName));

                SettingsRoot = Instantiate(await SettingsRootBundle.LoadAssetAsync(SettingsRootRef.AssetName) as GameObject, SettingsRootParent);
#if UNITY_EDITOR
            }
#endif

            GameObject.Find("SettingsRoot_BackButton").GetComponent<Button>().onClick.AddListener(DestroySettings);
            
            SavingRootObject.instance.LoadingAssetRoot.SetActive(false);
        }

        private void DestroySettings()
        {
            DestroySettingsAsync().Forget();
        }
        
        private async UniTaskVoid DestroySettingsAsync()
        {
            Destroy(SettingsRoot);

#if UNITY_EDITOR
            if (UseAssetBundlesInEditor)
            {
#endif                
                await SettingsRootBundle.UnloadAsync(true);
                await SettingsRootSharedBundle.UnloadAsync(true);
#if UNITY_EDITOR                
            }
#endif
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
                ChatManager.instance.SetAllowChatFocusing(false);
                
                LoadingManagerOnline.ClearCurrentIPPref();

                OnlineManager.instance.InOnlineGame = false;
                OnlineManager.instance.Disconnecting = true;
            
                OnlineManager.instance.DisconnectAsHost();
                OnlineManager.instance.DisconnectAndLoadMainMenu();
                return;
            }

            LoadingManager.instance.LoadScene(SceneNames.S_MainMenu, false);
#elif UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
        }

        private void Update()
        {
            if (Keyboard.current.f6Key.wasPressedThisFrame)
            {
                Instantiate(BotPlayerPrefab);
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}