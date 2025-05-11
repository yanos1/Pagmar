using System.Collections;
using DG.Tweening;
using Interfaces;
using UnityEngine;

// From Feel/DoTween

namespace Obstacles
{
    public class GuillotineTrap : MonoBehaviour, IKillPlayer,IResettable
    {
        [Header("Movement Settings")]
        public float downY = 0f;
        public float upY = 5f;
        public float downDuration = 0.2f;
        public float upDuration = 1.5f;
        public float delayBetweenCycles = 1f;

        [Header("Trigger Settings")]
        public string triggeringTag = "Player";

        private bool isActive = false;
        private Tween currentTween;
        private Vector3 startingPos;
        private bool isGoingDown = false;

        private void Start()
        {
            startingPos = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isActive && collision.CompareTag(triggeringTag))
            {
                isActive = true;
                StartCoroutine(GuillotineCycle());
            }
        }

        IEnumerator GuillotineCycle()
        {
            while (isActive)
            {
                // Go Down (Fast)
                currentTween = transform.DOMoveY(transform.position.y - downY, downDuration)
                    .SetEase(Ease.InQuad); // Fast, snappy drop
                isGoingDown = true;

                yield return currentTween.WaitForCompletion();
                yield return new WaitForSeconds(delayBetweenCycles);

                isGoingDown = false;
                currentTween = transform.DOMoveY(transform.position.y + upY, upDuration)
                    .SetEase(Ease.OutQuad); // Smooth slow return
                yield return currentTween.WaitForCompletion();
                yield return new WaitForSeconds(delayBetweenCycles);
            }
        }

        private void OnDisable()
        {
            currentTween?.Kill();
        }


        public bool IsDeadly()
        {
            return isActive;
        }

        public void ResetToInitialState()
        {
            isActive = false;
            transform.position = startingPos;
        }
    }
}

