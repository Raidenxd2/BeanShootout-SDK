using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class CurrentPlayer : NetworkBehaviour
    {
        public static CurrentPlayer instance;

        public PlayerMovement playerMovement;
        public BulletManager bulletManager;
        public PlayerCam playerCam;

        private void Start()
        {
            if (IsOwner)
            {
                Debug.Log("(CurrentPlayer/Owner) Setting instance of CurrentPlayer.instance to this instance");
                instance = this;
            }
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }
    }
}