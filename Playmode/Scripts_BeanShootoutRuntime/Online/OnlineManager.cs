using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Logging;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class OnlineManager : MonoBehaviour
    {
        public static OnlineManager instance;

        public bool InOnlineGame;

        public bool Disconnecting;

        [SerializeField] private GameObject NetworkManagerPrefab;

        private void Awake()
        {
            instance = this;
        }

#if KILLITMYSELF_FULL
        private void Start()
        {
            NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
            NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;

            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        }

        private void OnClientStarted()
        {
            NetworkManager.Singleton.ConnectionManager.OnDisconnect2 += OnDisconnect;
        }

        private void OnClientStopped(bool obj)
        {
            NetworkManager.Singleton.ConnectionManager.OnDisconnect2 -= OnDisconnect;
        }

        private void OnTransportFailure()
        {
            OnTransportFailureAsync().Forget();
        }

        private async UniTaskVoid OnTransportFailureAsync()
        {
            Destroy(GameObject.Find("PlayerInput(Clone)"));

            string errorString = await LocalizedStringReferences.instance.E_Online_Transport.GetLocalizedStringAsync();

            if (!string.IsNullOrEmpty(UnityTransportPrevLog.prevLog))
            {
                errorString += "\n<size=25>" + UnityTransportPrevLog.prevLog + "</size>";
                UnityTransportPrevLog.prevLog = null;
            }
            if (!string.IsNullOrEmpty(DebugLogPrev.prevLog))
            {
                errorString += "\n<size=25>" + DebugLogPrev.prevLog + "</size>";
                DebugLogPrev.prevLog = null;
            }
// #else
            // string errorString = "<size=25>A network error has occurred. (Transport)\nPlease check that the IP address you are connecting to is valid/correct and that you have an internet connection.\nCannot get error details in non Bean Shootout Unity project.</size>";
// #endif

            NetworkErrorManager.instance.ShowErrorAndDisconnect(errorString);
        }

        public void DisconnectAsHost()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            DisconnectPlayersRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void DisconnectPlayersRpc()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                return;
            }

            NetworkManager.Singleton.Shutdown();

            OnlineSceneManagement.instance.Stop();

            Debug.LogWarning("(OnlineManager) Host left.");

            NetworkManager.Singleton.Shutdown();

            Destroy(GameObject.Find("PlayerInput(Clone)"));

            HostLeftDialogAsync().Forget();
        }

        public void DisconnectAndLoadMainMenu()
        {
            Disconnecting = true;

            NetworkManager.Singleton.Shutdown();

            OnlineSceneManagement.instance.Stop();

            Destroy(GameObject.Find("PlayerInput(Clone)"));

            LoadingManager.instance.LoadScene(AddressableReferences.S_MainMenu);
        }

        private void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            if (Disconnecting)
            {
                Disconnecting = false;
                return;
            }
            
            Debug.Log("(OnlineManager) OnConnectionEvent for " + data.ClientId + " (" + data.EventType + ")");

            if (data.EventType == ConnectionEvent.ClientDisconnected && NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (data.ClientId == 0)
                {
                    Debug.LogWarning("(OnlineManager) Host left.");

                    HostLeftDialogAsync().Forget();
                }
            }
        }

        private void OnDisconnect(ulong arg0)
        {
            if (Disconnecting)
            {
                Disconnecting = false;
                return;
            }
            
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (arg0 == 0)
                {
                    Debug.LogWarning("(OnlineManager) Host left.");

                    HostLeftDialogAsync().Forget();
                }
            }
        }

        private async UniTaskVoid HostLeftDialogAsync()
        {
            if (ServerTick.instance != null)
            {
                ServerTick.instance.Stop();
            }

            Destroy(GameObject.Find("PlayerInput(Clone)"));
            
            NetworkErrorManager.instance.ShowErrorAndDisconnect(await LocalizedStringReferences.instance.Online_HostLeft.GetLocalizedStringAsync());
        }
#endif

        private void OnDestroy()
        {
            instance = null;
        }
    }
}