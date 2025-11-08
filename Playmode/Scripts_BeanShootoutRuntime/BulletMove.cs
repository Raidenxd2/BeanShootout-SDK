using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class BulletMove : NetworkBehaviour
    {
        private bool collisionEnabled;
        private Rigidbody rb;

        public int damage;
        public bool ShootBackwards;

        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();

            EnableCollision().Forget();
            DestroyWait().Forget();
        }

        public void FixedUpdate()
        {
            if (OnlineManager.instance.InOnlineGame && !IsServer)
            {
                return;
            }

            if (ShootBackwards)
            {
                rb.AddForce(-225f * -13 * -transform.forward.normalized, ForceMode.Force);
            }
            else
            {
                rb.AddForce(225f * 13 * transform.forward.normalized, ForceMode.Force);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collisionEnabled)
            {
                return;
            }
            
            if (collision.gameObject.CompareTag("Player"))
            {
                if (OnlineManager.instance.InOnlineGame)
                {
                    Debug.Log("damage");
                    collision.gameObject.GetComponent<HealthSystem>().DamageRpc(damage);
                    GetComponent<NetworkObject>().Despawn();
                }
                else
                {
                    collision.gameObject.GetComponent<HealthSystem>().Health -= damage;
                    Destroy(gameObject);
                }
            }
#if KILLITMYSELF_FULL
            else if (collision.gameObject.CompareTag("Bossfight/JumbotronScreen"))
            {
                BossfightAttacks.instance.Health -= damage;
                Destroy(gameObject);
            }
#endif
            else
            {
                if (OnlineManager.instance.InOnlineGame)
                {
                    GetComponent<NetworkObject>().Despawn();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        private async UniTask DestroyWait()
        {
            await UniTask.WaitForSeconds(2f);

            try
            {
                if (gameObject == null)
                {
                    return;
                }
            }
            catch
            {

            }
            
            try
            {
                if (OnlineManager.instance.InOnlineGame)
                {
                    GetComponent<NetworkObject>().Despawn();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            catch
            {

            }
        }

        private async UniTask EnableCollision()
        {
            await UniTask.WaitForSeconds(0.03f);
            collisionEnabled = true;
        }
    }
}