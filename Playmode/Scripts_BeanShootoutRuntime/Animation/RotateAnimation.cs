using UnityEngine;

namespace KillItMyself.Runtime.Animation
{
    public class RotateAnimation : MonoBehaviour
    {
        [SerializeField] private TransformType transformType;
        [SerializeField] private Vector3 RotateAngle;

        [Tooltip("For compatibility reasons")]
        [SerializeField] private bool ForceZAt300 = true;

        private RectTransform rt;

        private void Start()
        {
            if (ForceZAt300)
            {
                RotateAngle = new(0, 0, 300);
            }
            
            if (transformType == TransformType.RectTransform)
            {
                rt = GetComponent<RectTransform>();
            }
        }

        private void Update()
        {
            switch (transformType)
            {
                case TransformType.Transform:
                    transform.Rotate(RotateAngle * Time.unscaledDeltaTime);
                    break;

                case TransformType.RectTransform:
                    rt.Rotate(RotateAngle * Time.unscaledDeltaTime);
                    break;
            }
        }
    }

    public enum TransformType
    {
        Transform,
        RectTransform
    }
}