using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float timeToDestroy;

        private void Start()
        {
            DestroyWait().Forget();
        }

        private async UniTask DestroyWait()
        {
            await UniTask.WaitForSeconds(timeToDestroy);
            Destroy(gameObject);
        }
    }
}