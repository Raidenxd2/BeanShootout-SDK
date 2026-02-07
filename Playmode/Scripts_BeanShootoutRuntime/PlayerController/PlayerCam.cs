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

        public Transform playerModel;

        public bool canMoveCamera = true;
        private float xRotation;
        private float yRotation;

        [SerializeField] private PlayerInput playerControls;
        private InputAction CameraInput;
        
        private static bool playerHasJoined;

        [SerializeField] private Camera Camera;
        [SerializeField] private AudioListener AudioListener;
        [SerializeField] private UniversalAdditionalCameraData URPCamData;

        private void Start()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                Camera.enabled = false;
                AudioListener.enabled = false;
                return;
            }

            CameraInput = playerControls.actions["Camera"];

            URPCamData.renderPostProcessing = BetterPrefs.GetBool("PostProcessing", true);

            if (!playerHasJoined)
            {
#if KILLITMYSELF_FULL
                RotateTowardsPlayer1Camera.player = transform;
#endif
                playerHasJoined = true;
            }

            if (playerControls.currentControlScheme == "Gamepad")
            {
                sensX = 10 * BetterPrefs.GetInt("ControllerSettings_Sensitivity", 10);
                sensY = 10 * BetterPrefs.GetInt("ControllerSettings_Sensitivity", 10);
            }

            if (playerControls.devices[0].displayName.Contains("Keyboard") || playerControls.devices[0].displayName.Contains("Mouse"))
            {
                sensX = 2 * BetterPrefs.GetInt("KeyboardMouseSettings_MouseSensitivity", 1);
                sensY = 2 * BetterPrefs.GetInt("KeyboardMouseSettings_MouseSensitivity", 1);
            }

            if (BetterPrefs.GetBool("DeferredRendering", false))
            {
                URPCamData.SetRenderer(2);
            }
        }

        private void LateUpdate()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner || !canMoveCamera || PauseManager.instance.paused)
            {
                return;
            }

            Vector2 rotateDirection = CameraInput.ReadValue<Vector2>();

            yRotation += rotateDirection.x * sensX * Time.fixedDeltaTime;
            xRotation -= rotateDirection.y * sensY * Time.fixedDeltaTime;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //Rotate camera and playermodel
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

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