using UnityEngine;

namespace KillItMyself.Runtime
{
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager instance;

        public Transform[] SpawnPoints;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}