using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace KillItMyself.Runtime
{
    public class MouseCursor : MonoBehaviour
    {
        [SerializeField] private Image CursorImage;
        [SerializeField] private Sprite DefaultCursor;
        [SerializeField] private Sprite NotAllowedCursor;
        [SerializeField] private Sprite TextCursor;

        public bool HideCursorImage;
        public bool UseSystemMouseCursor;

        public static MouseCursor instance;

        private void Awake()
        {
            instance = this;
            Cursor.visible = false;

#if UNITY_ANDROID
            CursorImage.enabled = false;
#endif
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

        public void UpdateMouseCursorScale()
        {
            float val = BetterPrefs.GetFloat("KeyboardMouseSettings_MouseScale", 1);
            CursorImage.transform.localScale = new(val, val, val);
        }

        public void UpdateMouseCursor(MouseCursorType type)
        {
            switch (type)
            {
                case MouseCursorType.Default:
                    CursorImage.sprite = DefaultCursor;
                    break;
                case MouseCursorType.NotAllowed:
                    CursorImage.sprite = NotAllowedCursor;
                    break;
                case MouseCursorType.Text:
                    CursorImage.sprite = TextCursor;
                    break;
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }

    public enum MouseCursorType
    {
        Default = 0,
        NotAllowed = 1,
        Text = 2
    }
}