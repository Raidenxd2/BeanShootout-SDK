using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float sprintSpeed;

        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        public bool readyToJump;

        [Header("Camera")]
        public Camera playerCam;
        public float fovNormal = 75f;
        public float fovSprint = 80f;

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;
        public bool grounded;

        [Header("Other")]
        public Transform orientation;
        public GameObject playerModel;
        public LayerMask dontRenderLayer;
        public LayerMask spinnerLayer;
        public bool canMove = true;
        public bool IsOnKeyboardMouse;
        private float horizontalInput;
        private float verticalInput;
        private Vector3 moveDirection;
        private Rigidbody rb;
        Transform oldParent = null;
#if KILLITMYSELF_FULL
        public GameObject ShipLevel_OverrideCodeInteractUI;
        public GameObject ShipLevel_OverrideCodeUI;
#endif
        [SerializeField] private PlayerInput playerControls;
        [SerializeField] private Transform ControllerButtonsParent;
        [SerializeField] private GameObject XboxConrollerButtons;
        [SerializeField] private GameObject PlayStationButtons;
        [SerializeField] private GameObject NintendoButtons;
        [SerializeField] private GameObject GenericButtons;
        [SerializeField] private Transform PlayerLocationCircle;
        [SerializeField] private PlayerFade fade;

        private bool Respawning;

        private void Start()
        {
            PlayersJoined.instance.Players.Add(gameObject);

            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }

            // if (OnlineManager.instance.InOnlineGame)
            // {
            //     playerControls = GlobalPlayerInput.instance.playerInput;

            //     PauseInput.instance.playerInput = GlobalPlayerInput.instance.playerInput;
            // }

            oldParent = transform.parent;

            if (playerControls.currentControlScheme.Contains("Keyboard") || playerControls.currentControlScheme.Contains("Mouse"))
            {
                IsOnKeyboardMouse = true;
            }
            
            rb = gameObject.GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            ResetJump();
            
            Debug.Log("(PlayerMovement) Controller2: " + playerControls.devices[0].displayName);
            Debug.Log(playerControls.devices[0].name);

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

        private async UniTask Respawn()
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            fade.FadeIn();
            await UniTask.WaitForSeconds(1f);

            if (SpawnManager.instance != null)
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
                rb.linearDamping = groundDrag;
            }
            else
            {
                rb.linearDamping = 0;
            }

            PlayerLocationCircle.SetPositionAndRotation(new(1500.2f + transform.position.x, 1337.335f + transform.position.y, 1500 + transform.position.z), Quaternion.Euler(-90, playerCam.transform.rotation.eulerAngles.y - 90, 0));
        }

        private void MyInput()
        {
            Vector2 moveDirection;

            if (!canMove)
            {
                moveDirection = Vector2.zero;
                horizontalInput = 0;
                verticalInput = 0;
                return;
            }

            moveDirection = playerControls.actions["Movement"].ReadValue<Vector2>();
            horizontalInput = moveDirection.x;
            verticalInput = moveDirection.y;

            // When to jump
            if (playerControls.actions["Jump"].IsPressed() && readyToJump && grounded)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }

#if KILLITMYSELF_FULL
            if (playerControls.actions["Interact"].WasPressedThisFrame() && ShipLevel_OverrideCodeInteractUI.activeSelf && !ShipLevel_OverrideCodeUI.activeSelf)
            {
                canMove = false;

                Cursor.lockState = CursorLockMode.None;
                ShipLevel_OverrideCodeUI.SetActive(true);
            }
#endif
        }

#if KILLITMYSELF_FULL
        public void CloseShipOverrideCodeUI()
        {
            canMove = true;

            Cursor.lockState = CursorLockMode.Locked;
            ShipLevel_OverrideCodeUI.SetActive(false);
        }
#endif

        private void MovePlayer()
        {
            RaycastHit hitInfo = new();
            bool hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, playerHeight * 0.5f + 0.2f, spinnerLayer);

            if (hit)
            {
                transform.SetParent(hitInfo.transform);
            }
            else
            {
                if (oldParent != null)
                {
                    transform.SetParent(oldParent);
                }
            }

            // Calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            // On ground
            if (grounded)
            {
                if (playerControls.actions["Sprint"].IsPressed())
                {
                    playerCam.fieldOfView = fovSprint;
                    rb.AddForce(moveDirection.normalized * sprintSpeed * 10f, ForceMode.Force);
                }
                else
                {
                    playerCam.fieldOfView = fovNormal;
                    rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                }
            }
            // In air
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }

        public void Jump()
        {
            //Reset Y velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        public void ResetJump()
        {
            readyToJump = true;
        }
    }
}