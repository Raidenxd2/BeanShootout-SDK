using UnityEngine;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    public class GunLagBehind : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;

        [SerializeField] private PlayerInput playerControls;
        private InputAction CameraInput;

        [SerializeField] private float swayAmount;
        [SerializeField] private float maxSwayAmount;
        [SerializeField] private float smoothAmount;

        private Quaternion initialRotation;

        private void Start()
        {
            initialRotation = rt.localRotation;

            CameraInput = playerControls.actions["Camera"];
        }

        private void Update()
        {
            Vector2 rotateDirection = CameraInput.ReadValue<Vector2>();

            rotateDirection = new Vector2(Mathf.Clamp(rotateDirection.x, -maxSwayAmount, maxSwayAmount), Mathf.Clamp(rotateDirection.y, -maxSwayAmount, maxSwayAmount));

            Quaternion targetRotationX = Quaternion.AngleAxis(-rotateDirection.x, Vector3.up);
            Quaternion targetRotationY = Quaternion.AngleAxis(rotateDirection.y, Vector3.right);
            Quaternion targetRotation = initialRotation * targetRotationX * targetRotationY;

            rt.localRotation = Quaternion.Lerp(rt.localRotation, targetRotation, Time.deltaTime * smoothAmount);
        }
    }
}