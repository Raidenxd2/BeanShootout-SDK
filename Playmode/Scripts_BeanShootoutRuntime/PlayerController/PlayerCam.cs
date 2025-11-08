using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(UniversalAdditionalCameraData))]
    [RequireComponent(typeof(AudioListener))]
    public class PlayerCam : NetworkBehaviour
    {
        public float sensX;
        public float sensY;

        public Transform orientation;
        public Transform playerModel;

        public bool canMoveCamera = true;
        private float xRotation;
        private float yRotation;

        [SerializeField] private PlayerInput playerControls;

        private static bool playerHasJoined;

        private void Start()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                gameObject.GetComponent<Camera>().enabled = false;
                gameObject.GetComponent<AudioListener>().enabled = false;
                return;
            }

            // if (OnlineManager.instance.InOnlineGame)
            // {
            //     playerControls = GlobalPlayerInput.instance.playerInput;
            // }

            GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = BetterPrefs.GetBool("PostProcessing", true);

            if (!playerHasJoined)
            {
                gameObject.tag = "Player1Camera";
                playerHasJoined = true;
            }

            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            if (playerControls.currentControlScheme == "Gamepad")
            {
                sensX = 10 * BetterPrefs.GetInt("ControllerSettings_Sensitivity", 10);
                sensY = 10 * BetterPrefs.GetInt("ControllerSettings_Sensitivity", 10);
            }

            if (playerControls.devices[0].displayName.Contains("Keyboard") || playerControls.devices[0].displayName.Contains("Mouse"))
            {
                sensX = 10 * BetterPrefs.GetInt("KeyboardMouseSettings_MouseSensitivity", 1);
                sensY = 10 * BetterPrefs.GetInt("KeyboardMouseSettings_MouseSensitivity", 1);
            }
        }

        private void LateUpdate()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner || !canMoveCamera || PauseManager.instance.paused)
            {
                return;
            }

            Vector2 rotateDirection = playerControls.actions["Camera"].ReadValue<Vector2>();

            yRotation += rotateDirection.x * sensX * Time.fixedDeltaTime;
            xRotation -= rotateDirection.y * sensY * Time.fixedDeltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //Rotate camera and orientation
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);

            playerModel.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        public static void ChangePlayerHasJoined()
        {
            playerHasJoined = false;
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        public static void ResetValues()
        {
            playerHasJoined = false;
        }
#endif
    }
}