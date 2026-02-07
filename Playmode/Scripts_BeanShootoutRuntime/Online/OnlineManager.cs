using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class OnlineManager : MonoBehaviour
    {
        public static OnlineManager instance;

        public bool InOnlineGame;
        public bool Host_InGame;

        public bool Connecting;
        public bool Disconnecting;

        public bool IgnoreErrors;

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

            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
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
            if (IgnoreErrors)
            {
                return;
            }

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
            
            Connecting = false;

            NetworkErrorManager.instance.ShowErrorAndDisconnect(errorString);
        }

        public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                response.Approved = true;
            }

            if (CurrentBannedPlayers.current.playerUniques.Contains(request.Payload.ToString()))
            {
                response.Approved = false;
                response.Reason = "You were kicked from the server: You have been permanently banned from this server.";
                return;
            }

            if (Host_InGame)
            {
                response.Approved = false;
                response.Reason = "You were kicked from the server: Cannot join in progress game.";
                return;
            }

            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
            response.Position = Vector3.zero;
            response.Rotation = Quaternion.identity;
            response.Pending = false;
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

            OnlineSceneManagement.instance.Stop();

            BeanLogger.LogWarning("Host left.", this);

            Destroy(GameObject.Find("PlayerInput(Clone)"));

            HostLeftDialogAsync(true).Forget();
        }

        public void DisconnectAndLoadMainMenu()
        {
            Disconnecting = true;
            Connecting = false;
            Host_InGame = false;

            NetworkManager.Singleton.Shutdown();

            OnlineSceneManagement.instance.Stop();

            Destroy(GameObject.Find("PlayerInput(Clone)"));

            LoadingManager.instance.LoadScene(SceneNames.S_MainMenu, false);
        }

        private void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
        {
            if (IgnoreErrors)
            {
                return;
            }

            if (Disconnecting)
            {
                Disconnecting = false;
                return;
            }
            
            BeanLogger.Log("OnConnectionEvent for " + data.ClientId + " (" + data.EventType + ")", this);

            if (data.EventType == ConnectionEvent.ClientDisconnected && NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (data.ClientId == 0)
                {
                    BeanLogger.LogWarning("Host left.", this);

                    HostLeftDialogAsync().Forget();
                }
            }
        }

        private void OnDisconnect(ulong arg0)
        {
            if (IgnoreErrors)
            {
                return;
            }

            if (Disconnecting)
            {
                Disconnecting = false;
                return;
            }
            
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (arg0 == 0)
                {
                    BeanLogger.LogWarning("Host left.", this);

                    HostLeftDialogAsync().Forget();
                }
            }
        }

        private async UniTaskVoid HostLeftDialogAsync(bool forceDisconnect = false)
        {
            if (IgnoreErrors && !forceDisconnect)
            {
                return;
            }

            if (ServerTick.instance)
            {
                ServerTick.instance.Stop();
            }

            Destroy(GameObject.Find("PlayerInput(Clone)"));
            
            BeanLogger.Log(NetworkManager.Singleton.DisconnectReason, this);

            if (Connecting)
            {
                NetworkErrorManager.instance.ShowErrorAndDisconnect(await LocalizedStringReferences.instance.Online_FailedToConnect.GetLocalizedStringAsync());
                return;
            }
            
            if (!string.IsNullOrEmpty(NetworkManager.Singleton.DisconnectReason))
            {
                if (NetworkManager.Singleton.DisconnectReason.Equals("SyncError"))
                {
                    NetworkErrorManager.instance.ShowErrorAndDisconnect(await LocalizedStringReferences.instance.Online_FailedToSyncData.GetLocalizedStringAsync());
                }
                else
                {
                    NetworkErrorManager.instance.ShowErrorAndDisconnect(NetworkManager.Singleton.DisconnectReason + "\n\nReturning to main menu...");
                }
            }
            else
            {
                NetworkErrorManager.instance.ShowErrorAndDisconnect(await LocalizedStringReferences.instance.Online_HostLeft.GetLocalizedStringAsync());
            }
        }
#endif

        private void OnDestroy()
        {
#if KILLITMYSELF_FULL
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
                NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;

                NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
                NetworkManager.Singleton.OnClientStopped -= OnClientStopped;

                NetworkManager.Singleton.ConnectionApprovalCallback = null;
            }
#endif

            instance = null;
        }
    }
}