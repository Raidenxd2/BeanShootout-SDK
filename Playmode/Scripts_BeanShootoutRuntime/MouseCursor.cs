using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KillItMyself.Runtime
{
    public class MouseCursor : MonoBehaviour
    {
        [SerializeField] private Image CursorImage;

        public bool HideCursorImage;
        public bool UseSystemMouseCursor;

        public static MouseCursor instance;

        private void Awake()
        {
            instance = this;
            Cursor.visible = false;
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        private void Update()
        {
            if (Cursor.lockState == CursorLockMode.Locked && !CursorImage.enabled)
            {
                return;
            }

            if (UseSystemMouseCursor)
            {
                CursorImage.enabled = false;

                if (Cursor.lockState == CursorLockMode.Locked || HideCursorImage)
                {
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.visible = true;
                }

                return;
            }

            if (Cursor.lockState == CursorLockMode.Locked || HideCursorImage)
            {
                CursorImage.enabled = false;
            }
            else
            {
                CursorImage.enabled = true;
            }
        }

        private void LateUpdate()
        {
            if (!CursorImage.enabled)
            {
                return;
            }

            transform.position = Mouse.current.position.ReadValue();
        }
#endif

        private void OnDestroy()
        {
            instance = null;
        }
    }
}