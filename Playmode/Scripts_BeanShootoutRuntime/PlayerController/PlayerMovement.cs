using Cysharp.Threading.Tasks;
using KillItMyself.Runtime.Animation;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        private bool readyToJump;

        [Header("Camera")]
        public Camera playerCam;

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;
        public bool grounded;

        [Header("Other")]
        [SerializeField] private bool isBot;
#if KILLITMYSELF_FULL
        [SerializeField] private BotPlayer bot;
#endif
        public Transform playerModel;
        // public LayerMask dontRenderLayer;
        public LayerMask spinnerLayer;
        public bool canMove = true;
        public bool IsOnKeyboardMouse;
        private float horizontalInput;
        private float verticalInput;
        private Vector3 moveDirection;
        [SerializeField] private Rigidbody rb;
        Transform oldParent;
#if KILLITMYSELF_FULL
        public GameObject ShipLevel_OverrideCodeInteractUI;
        public GameObject ShipLevel_OverrideCodeUI;
        public GameObject HotelLevel_LeverInteractUI; 
        public GameObject HotelLevel_CodeInputInteractUI;
        public GameObject HotelLevel_CodeInputUI;
#endif
        [SerializeField] private PlayerInput playerControls;
        private InputAction MovementInput;
        public InputAction JumpInput;
        private InputAction InteractInput;
        private InputAction SprintInput;
        [SerializeField] private Transform ControllerButtonsParent;
        [SerializeField] private GameObject XboxConrollerButtons;
        [SerializeField] private GameObject PlayStationButtons;
        [SerializeField] private GameObject NintendoButtons;
        [SerializeField] private GameObject GenericButtons;
        [SerializeField] private Transform PlayerLocationCircle;
        [SerializeField] private PlayerFade fade;

        public BulletManager bulletManager;
        [SerializeField] private PlayerCam playerCamComponent;

        [SerializeField] private GameObject DeviceDisconnectedUI;
        
        private CursorLockMode prevCursorLockMode;
        private bool prevCanMove;
        private bool prevCannotShootNoMatterWhat;
        private bool prevCanShoot;
        private bool prevCanMoveCamera;
        private Vector3 prevVel;

        private bool Respawning;

        private void Start()
        {
            PlayersJoined.instance.Players.Add(gameObject);

            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }

#if KILLITMYSELF_FULL            
            if (SceneManager.GetActiveScene().name == "Secret_BossfightPhase1")
            {
                BossfightAttacks.instance.AddHealthForPlayer();
            }
#endif

            oldParent = transform.parent;
            
            rb.freezeRotation = true;

            ResetJump();
            
#if KILLITMYSELF_FULL
            if (isBot)
            {
                bot.Init();
                return;
            }
#endif
            
            if (playerControls.currentControlScheme.Contains("Keyboard") || playerControls.currentControlScheme.Contains("Mouse"))
            {
                IsOnKeyboardMouse = true;
            }

            if (CommandLineArgs.VerboseLoggingEnabled)
            {
                BeanLogger.Log("Controller2: " + playerControls.devices[0].displayName, this);
                BeanLogger.Log(playerControls.devices[0].name, this);
            }

            if (playerControls.devices[0].displayName.Contains("Xbox"))
            {
                Instantiate(XboxConrollerButtons, ControllerButtonsParent);
            }
            else if (playerControls.devices[0].displayName.Contains("DualSense") || playerControls.devices[0].displayName.Contains("DualShock") || playerControls.devices[0].name.Contains("DualShock"))
            {
                Instantiate(PlayStationButtons, ControllerButtonsParent);
            }
            else if (playerControls.devices[0].displayName.Contains("Nintendo") || playerControls.devices[0].displayName.Contains("Pro Controller") || playerControls.devices[0].name.Contains("Switch") || playerControls.devices[0].name.Contains("ProController"))
            {
                Instantiate(NintendoButtons, ControllerButtonsParent);
            }
            else if (playerControls.devices[0].name.Contains("Gamepad"))
            {
                // Instantiate(GenericButtons, ControllerButtonsParent);
            }

            playerControls.deviceRegainedEvent.AddListener(OnDeviceReconnect);

            MovementInput = playerControls.actions["Movement"];
            JumpInput = playerControls.actions["Jump"];
            InteractInput = playerControls.actions["Interact"];
            SprintInput = playerControls.actions["Sprint"];

#if KILLITMYSELF_FULL
            if (GameObject.Find("BrokenArcadeMachineSound"))
            {
                GameObject.Find("BrokenArcadeMachineSound").GetComponent<AudioSource>().volume = 0.2f;
            }
#endif
        }

        private void FixedUpdate()
        {
            if (Respawning || OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }

            if (transform.position.y <= -100 && !Respawning)
            {
                Respawning = true;
                Respawn().Forget();
            }

            MovePlayer();
        }

        private string deviceName;
        public void OnDeviceDisconnect(PlayerInput playerInput)
        {
            deviceName = playerInput.devices[0].displayName;
            BeanLogger.LogWarning("Device " + deviceName + " has disconnected.", this);
            DeviceDisconnectedUI.SetActive(true);
        }

        public void OnDeviceReconnect(PlayerInput playerInput)
        {
            BeanLogger.LogWarning("Device " + deviceName + " has reconnected.", this);
            DeviceDisconnectedUI.GetComponent<WindowAnimation>().Close();
        }

        private async UniTask Respawn()
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            fade.FadeIn();
            await UniTask.WaitForSeconds(1f);

            if (SpawnManager.instance)
            {
                rb.position = SpawnManager.instance.SpawnPoints[Random.Range(0, SpawnManager.instance.SpawnPoints.Length)].position;
            }
            else
            {
                rb.position = Vector3.zero;
            }

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            Respawning = false;
            rb.useGravity = true;
            fade.FadeOut();
        }

        private void Update()
        {
            if (Respawning || OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }

            // Ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            MyInput();
            SpeedControl();

            // Handle drag
            if (grounded)
            {
                rb.linearDamping = GameSettings.MovementSettings.groundDrag;
            }
            else
            {
                rb.linearDamping = 0;
            }

            PlayerLocationCircle.SetPositionAndRotation(new(1500.2f + transform.position.x, 1337.335f + transform.position.y, 1500 + transform.position.z), Quaternion.Euler(-90, playerCam.transform.rotation.eulerAngles.y - 90, 0));
        }

        private void MyInput()
        {
            if (!canMove)
            {
                horizontalInput = 0;
                verticalInput = 0;
                return;
            }

            Vector2 moveDirection = MovementInput.ReadValue<Vector2>();
            horizontalInput = moveDirection.x;
            verticalInput = moveDirection.y;

            // When to jump
            if (JumpInput.IsPressed() && readyToJump && grounded)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), GameSettings.MovementSettings.jumpCooldown);
            }

#if KILLITMYSELF_FULL
            if (InteractInput.WasPressedThisFrame() && ShipLevel_OverrideCodeInteractUI.activeSelf && !ShipLevel_OverrideCodeUI.activeSelf)
            {
                PreventPlayerFromDoingAnything();
                Cursor.lockState = CursorLockMode.None;
                
                ShipLevel_OverrideCodeUI.SetActive(true);
            }

            if (InteractInput.WasPressedThisFrame() && HotelLevel_LeverInteractUI.activeSelf)
            {
                HotelLevel_LeverInteract.instance.LeverInteract();
                HotelLevel_LeverInteractUI.SetActive(false);
            }

            if (InteractInput.WasPressedThisFrame() && HotelLevel_CodeInputInteractUI.activeSelf)
            {
                PreventPlayerFromDoingAnything();
                Cursor.lockState = CursorLockMode.None;
                
                HotelLevel_CodeInputUI.SetActive(true);
            }
#endif
        }

#if KILLITMYSELF_FULL
        public void CloseShipOverrideCodeUI()
        {
            LetPlayerDoAnything();

            Cursor.lockState = CursorLockMode.Locked;
            ShipLevel_OverrideCodeUI.SetActive(false);
            HotelLevel_CodeInputUI.SetActive(false);
        }
#endif

        private void MovePlayer()
        {
            bool hit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, playerHeight * 0.5f + 0.2f, spinnerLayer);

            if (hit)
            {
                transform.SetParent(hitInfo.transform);
            }
            else
            {
                if (oldParent)
                {
                    transform.SetParent(oldParent);
                }
            }

            // Calculate movement direction
            moveDirection = playerModel.forward * verticalInput + playerModel.right * horizontalInput;

            // On ground
            if (grounded)
            {
                if (SprintInput.IsPressed())
                {
                    playerCam.fieldOfView = GameSettings.MovementSettings.fovSprint;
                    rb.AddForce(moveDirection.normalized * (GameSettings.MovementSettings.sprintSpeed * 10f), ForceMode.Force);
                }
                else
                {
                    playerCam.fieldOfView = GameSettings.MovementSettings.fovNormal;
                    rb.AddForce(moveDirection.normalized * (GameSettings.MovementSettings.moveSpeed * 10f), ForceMode.Force);
                }
            }
            // In air
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * (GameSettings.MovementSettings.moveSpeed * 10f * GameSettings.MovementSettings.airMultiplier), ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > GameSettings.MovementSettings.moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * GameSettings.MovementSettings.moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        private void Jump()
        {
            //Reset Y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(transform.up * GameSettings.MovementSettings.jumpForce, ForceMode.Impulse);
        }

        public void ResetJump()
        {
            readyToJump = true;
        }

        public void PreventPlayerFromDoingAnything()
        {
            // prevCanMove = canMove;
            // prevVel = GetComponent<Rigidbody>().linearVelocity;
            // prevCanShoot = bulletManager.CanShoot;
            // prevCanMoveCamera = playerCamComponent.canMoveCamera;
            // prevCannotShootNoMatterWhat = bulletManager.CannotShootNoMatterWhat;
            canMove = false;
            rb.linearVelocity = Vector3.zero;
            // bulletManager.CanShoot = false;
            bulletManager.CannotShootNoMatterWhat = true;
            playerCamComponent.canMoveCamera = false;
        }

        public void LetPlayerDoAnything()
        {
            canMove = true;
            // rb.linearVelocity = prevVel;
            // bulletManager.CanShoot = true;
            bulletManager.CannotShootNoMatterWhat = false;
            playerCamComponent.canMoveCamera = true;
        }
    }
}