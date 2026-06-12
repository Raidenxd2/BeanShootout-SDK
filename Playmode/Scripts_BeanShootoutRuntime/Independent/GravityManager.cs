using UnityEngine;

namespace KillItMyself.Independent
{
    public class GravityManager : MonoBehaviour
    {
        public Vector3 NewGravity = new(0, -15.82f, 0);

        private void Start()
        {
            Physics.gravity = NewGravity;
        }
    }
}