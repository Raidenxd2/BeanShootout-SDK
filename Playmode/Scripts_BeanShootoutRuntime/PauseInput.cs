using Unity.Netcode;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    public class PauseInput : NetworkBehaviour
    {
        public PlayerInput playerInput;
        private InputAction PauseInputA;

        private void Start()
        {
            PauseInputA = playerInput.actions["Pause"];
        }

        private void Update()
        {
            if (PauseInputA.WasPressedThisFrame())
            {
                PauseManager.instance.PauseOrUnpause();
            }
        }
    }
}