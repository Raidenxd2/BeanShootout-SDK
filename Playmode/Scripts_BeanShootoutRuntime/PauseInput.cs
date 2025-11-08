using Unity.Netcode;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    public class PauseInput : NetworkBehaviour
    {
        public PlayerInput playerInput;

        public static PauseInput instance;

        private void Awake()
        {
            if (!IsOwner)
            {
                return;
            }

            instance = this;
        }

        private new void OnDestroy()
        {
            instance = null;
        }

        private void Update()
        {
            if (playerInput.actions["Pause"].WasPressedThisFrame())
            {
                PauseManager.instance.PauseOrUnpause();
            }
        }
    }
}