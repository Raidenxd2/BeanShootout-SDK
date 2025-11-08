using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class Recoil : NetworkBehaviour
    {
        [SerializeField] private RectTransform rt;

        [SerializeField] private float speed;
        [SerializeField] private Vector3 rot;

        private void Update()
        {
            rt.localRotation = Quaternion.Slerp(rt.localRotation, Quaternion.Euler(0, -107.78f, 0), speed * Time.deltaTime);
        }

        public void FireRecoil()
        {
            rt.localRotation = Quaternion.Euler(rt.localRotation.eulerAngles + rot);
        }

        [Rpc(SendTo.Everyone)]
        public void FireRecoilRpc()
        {
            FireRecoil();
        }

        public void UpdateValuesForCurrentGun(GunSO gun)
        {
            speed = gun.RecoilSpeed;
            rot = gun.RecoilRot;
        }
    }
}