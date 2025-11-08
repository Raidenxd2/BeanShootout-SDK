using UnityEngine;

namespace KillItMyself.Runtime
{
    public class UIPlaySound : MonoBehaviour
    {
        public void PlaySound(int sound)
        {
#if KILLITMYSELF_FULL
            SoundManager.instance.PlaySound(sound);
#endif
        }
    }
}