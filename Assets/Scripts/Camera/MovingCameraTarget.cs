using System;
using UnityEngine;
using DG.Tweening;
using Interfaces;
using Managers; // Make sure DOTween is imported

namespace Camera
{
    public class MovingCameraTarget : MonoBehaviour, IResettable
    {
        [SerializeField] private float duration = 2.5f;
        [SerializeField] private Vector3 targetScale = new Vector3(16.1f, 8.894533f, 0);

        private Vector3 originalScale;
        private Tween scaleTween;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterSlowMotion, ReduceScaleOverDuration);
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterSlowMotion, ReduceScaleOverDuration);
        }

        public void ReduceScaleOverDuration(object o)
        {
            // Kill any running tween to avoid conflicts
            print("enter reduce sclae");
            scaleTween?.Kill();

            // Tween to target scale
            scaleTween = transform.DOScale(targetScale, duration);
            Invoke(nameof(ResetToInitialState), 4);
        }

        public void ResetToInitialState()
        {
            // Kill any running tween to avoid conflicts
            scaleTween?.Kill();

            // Tween back to original scale
            scaleTween = transform.DOScale(originalScale, duration);
        }
    }
}