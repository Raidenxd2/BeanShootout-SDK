using Unity.Netcode;

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
                BeanLogger.Log("(Owner) Setting instance of CurrentPlayer.instance to this instance", this);
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