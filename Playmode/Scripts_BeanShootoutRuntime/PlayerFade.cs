using UnityEngine;

namespace KillItMyself.Runtime
{
    public class PlayerFade : MonoBehaviour
    {
        [SerializeField] private Animator FadeAnimator;

        public void FadeIn()
        {
            FadeAnimator.Play("FadeIn");
        }

        public void FadeOut()
        {
            FadeAnimator.Play("FadeOut");
        }
    }
}