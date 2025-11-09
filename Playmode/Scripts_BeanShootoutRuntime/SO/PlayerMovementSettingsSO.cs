using UnityEngine;

namespace KillItMyself.Runtime
{
    [CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "KillItMyself/PlayerMovement Settings", order = 1)]
    public class PlayerMovementSettingsSO : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed;
        public float sprintSpeed;

        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;

        [Header("Camera")]
        public float fovNormal = 75f;
        public float fovSprint = 80f;
    }
}