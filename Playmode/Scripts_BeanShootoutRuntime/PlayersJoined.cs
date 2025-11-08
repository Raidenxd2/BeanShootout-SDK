using System.Collections.Generic;
using UnityEngine;

namespace KillItMyself.Runtime
{
    public class PlayersJoined : MonoBehaviour
    {
        public static PlayersJoined instance;

        public List<GameObject> Players;

        private void Awake()
        {
            instance = this;

            Players = new();
        }

        private void OnDestroy()
        {
            Players.Clear();
            instance = null;
        }
    }
}