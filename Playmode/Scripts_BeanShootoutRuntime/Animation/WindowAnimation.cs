using Cysharp.Threading.Tasks;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace KillItMyself.Runtime.Animation
{
    public class WindowAnimation : MonoBehaviour
    {
        [SerializeField] private Ease UIEase1;
        [SerializeField] private Ease UIEase2;

        [SerializeField] private float duration;

        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private bool DoAnimationOnEnable = true;

        [SerializeField] private GameObject Canvas;
        
        private void OnEnable()
        {
            if (!DoAnimationOnEnable)
            {
                return;
            }
            
            OpenAnimationInternal().Forget();
        }

        public async UniTask OpenAnimationAsync()
        {
            await OpenAnimationInternal();
        }

        private async UniTask OpenAnimationInternal()
        {
            transform.localScale = new Vector3(0, 0, 0);
            
            LMotion.Create(Vector3.zero, Vector3.one, duration)
                .WithEase(UIEase1)
                .WithScheduler(MotionScheduler.TimeUpdateIgnoreTimeScale)
                .BindToLocalScale(transform);

            await LMotion.Create(0f, 1f, duration)
                .WithEase(UIEase1)
                .WithScheduler(MotionScheduler.TimeUpdateIgnoreTimeScale)
                .Bind(x => canvasGroup.alpha = x);
        }

        public void Close()
        {
            CloseAnimationInternal().Forget();
        }

        public async UniTask CloseAsync()
        {
            await CloseAnimationInternal();
        }

        private async UniTask CloseAnimationInternal()
        {
            LMotion.Create(Vector3.one, Vector3.zero, duration)
                .WithEase(UIEase2)
                .WithScheduler(MotionScheduler.TimeUpdateIgnoreTimeScale)
                .WithOnComplete(() => gameObject.SetActive(false))
                .BindToLocalScale(transform);

            await LMotion.Create(1f, 0f, duration)
                .WithEase(UIEase2)
                .WithScheduler(MotionScheduler.TimeUpdateIgnoreTimeScale)
                .WithOnComplete(() => gameObject.SetActive(false))
                .Bind(x => canvasGroup.alpha = x);

            if (Canvas)
            {
                Canvas.SetActive(false);
            }
        }
    }
}