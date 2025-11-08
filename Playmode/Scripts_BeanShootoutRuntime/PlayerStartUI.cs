using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KillItMyself.Runtime
{
    public class PlayerStartUI : NetworkBehaviour
    {
        [SerializeField] private PlayerInput playerControls;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private BulletManager bullet;
        [SerializeField] private Recoil recoil;

        [SerializeField] private GameObject PlayerStartUIRoot;

        [SerializeField] private Image GunImage;
        [SerializeField] private TMP_Text GunNameText;

        [SerializeField] private Image PlayerColorImage;
        [SerializeField] private Image PlayerVisorColorImage;

        [SerializeField] private GameObject XboxControllerIcons;
        [SerializeField] private GameObject PlayStationControllerIcons;
        [SerializeField] private GameObject UniversalControllerIcons;
        [SerializeField] private GameObject NintendoButtons;
        [SerializeField] private GameObject GenericButtons;
        [SerializeField] private Transform ControllerIconsParent;

        [SerializeField] private List<GunSO> guns = new();
        private int currentIndex;

        [SerializeField] private Color[] colors;
        private int PlayerColorCurrentIndex;
        private int PlayerVisorColorCurrentIndex;

        [SerializeField] private MeshRenderer playerRenderer;
        [SerializeField] private MeshRenderer playerLocationRenderer;

        private void Start()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }

            // if (OnlineManager.instance.InOnlineGame)
            // {
            //     playerControls = GlobalPlayerInput.instance.playerInput;
            // }

            GetComponent<Rigidbody>().position = GameObject.Find("PlayerSpawnBox").transform.position;

#if KILLITMYSELF_FULL
            if (CustomGunManager.AreGunModsLoaded)
            {
                guns.AddRange(CustomGunManager.ModSo);
            }
#endif

            if (playerControls.devices[0].displayName.Contains("Xbox"))
            {
                Instantiate(XboxControllerIcons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
            else if (playerControls.devices[0].displayName.Contains("DualSense"))
            {
                Instantiate(PlayStationControllerIcons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
            else if (playerControls.devices[0].displayName.Contains("Nintendo") || playerControls.devices[0].displayName.Contains("Pro Controller") || playerControls.devices[0].name.Contains("Switch") || playerControls.devices[0].name.Contains("ProController"))
            {
                Instantiate(NintendoButtons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
            else if (playerControls.devices[0].name.Contains("Gamepad"))
            {
                // Instantiate(GenericButtons, ControllerIconsParent);
                Instantiate(UniversalControllerIcons, ControllerIconsParent);
            }
        }

        public void JoinGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            bullet.gun = guns[currentIndex];
            bullet.BulletManagerInit();
            bullet.CanShoot = true;

            recoil.UpdateValuesForCurrentGun(guns[currentIndex]);

            playerRenderer.materials[0].color = colors[PlayerColorCurrentIndex];
            playerRenderer.materials[1].color = colors[PlayerVisorColorCurrentIndex];
            playerLocationRenderer.material.color = colors[PlayerColorCurrentIndex];

            PlayerStartUIRoot.SetActive(false);
            GetComponent<PlayerStartUI>().enabled = false;

            if (SpawnManager.instance != null)
            {
                GetComponent<Rigidbody>().position = SpawnManager.instance.SpawnPoints[Random.Range(0, SpawnManager.instance.SpawnPoints.Length)].position;
            }
            else
            {
                GetComponent<Rigidbody>().position = Vector3.zero;
            }
        }

        public void GunUp()
        {
            currentIndex++;
            if (currentIndex >= guns.Count - 1)
            {
                currentIndex = guns.Count - 1;
            }

            GunImage.sprite = guns[currentIndex].Image;
            GunNameText.text = guns[currentIndex].GunName;
        }

        public void GunDown()
        {
            currentIndex--;
            if (currentIndex <= 0)
            {
                currentIndex = 0;
            }

            GunImage.sprite = guns[currentIndex].Image;
            GunNameText.text = guns[currentIndex].GunName;
        }

        public void PlayerColorSelectUp()
        {
            PlayerColorCurrentIndex++;
            if (PlayerColorCurrentIndex >= colors.Length - 1)
            {
                PlayerColorCurrentIndex = colors.Length - 1;
            }

            PlayerColorImage.color = colors[PlayerColorCurrentIndex];
        }

        public void PlayerColorSelectDown()
        {
            PlayerColorCurrentIndex--;
            if (PlayerColorCurrentIndex <= 0)
            {
                PlayerColorCurrentIndex = 0;
            }

            PlayerColorImage.color = colors[PlayerColorCurrentIndex];
        }

        public void PlayerVisorColorSelectUp()
        {
            PlayerVisorColorCurrentIndex++;
            if (PlayerVisorColorCurrentIndex >= colors.Length - 1)
            {
                PlayerVisorColorCurrentIndex = colors.Length - 1;
            }

            PlayerVisorColorImage.color = colors[PlayerVisorColorCurrentIndex];
        }

        public void PlayerVisorColorSelectDown()
        {
            PlayerVisorColorCurrentIndex--;
            if (PlayerVisorColorCurrentIndex <= 0)
            {
                PlayerVisorColorCurrentIndex = 0;
            }

            PlayerVisorColorImage.color = colors[PlayerVisorColorCurrentIndex];
        }

        private void Update()
        {
            if (OnlineManager.instance.InOnlineGame && !IsOwner)
            {
                return;
            }
            
            if (playerMovement.IsOnKeyboardMouse)
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (playerControls.actions["Jump"].WasPressedThisFrame())
            {
                JoinGame();
            }
            if (playerControls.actions["DPadUp"].WasPressedThisFrame())
            {
                GunUp();
            }
            if (playerControls.actions["DPadDown"].WasPressedThisFrame())
            {
                GunDown();
            }
            if (playerControls.actions["DPadRight"].WasPerformedThisFrame())
            {
                PlayerColorSelectUp();
            }
            if (playerControls.actions["DPadLeft"].WasPressedThisFrame())
            {
                PlayerColorSelectDown();
            }
            if (playerControls.actions["ButtonWest"].WasPressedThisFrame())
            {
                PlayerVisorColorSelectUp();
            }
            if (playerControls.actions["ButtonNorth"].WasPressedThisFrame())
            {
                PlayerVisorColorSelectDown();
            }
        }
    }
}