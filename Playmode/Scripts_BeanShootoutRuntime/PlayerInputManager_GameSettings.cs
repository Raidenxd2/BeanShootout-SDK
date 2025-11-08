using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KillItMyself.Runtime
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerInputManager_GameSettings : MonoBehaviour
    {
        private PlayerInputManager pim;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);

            pim = GetComponent<PlayerInputManager>();

            pim.m_MaxPlayerCount = GameSettings.MaxPlayers;

            if (GameSettings.FullscreenNoOtherPlayers)
            {
                pim.m_FixedNumberOfSplitScreens = -1;
            }
            else
            {
                pim.m_FixedNumberOfSplitScreens = GameSettings.MaxPlayers;
            }
        }
    }
}