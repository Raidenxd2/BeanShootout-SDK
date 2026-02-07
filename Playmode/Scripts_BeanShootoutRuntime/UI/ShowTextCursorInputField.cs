using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(TMP_InputField))]
    public class ShowTextCursorInputField : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private TMP_InputField inputField;

        private void Start()
        {
            inputField = GetComponent<TMP_InputField>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (inputField.interactable)
            {
                MouseCursor.instance.UpdateMouseCursor(MouseCursorType.Text);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (inputField.interactable)
            {
                MouseCursor.instance.UpdateMouseCursor(MouseCursorType.Default);
            }
        }
    }
}