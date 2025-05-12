using System.Collections;
using DG.Tweening;
using Interfaces;
using SpongeScene;
using Triggers;
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

        [SerializeField] private Trigger _trigger;

        private bool isActive = false;
        private Coroutine routine;
        private Tween currentTween;
        private Vector3 startingPos;
        private bool isGoingDown = false;

        private void Start()
        {
            startingPos = transform.position;
            StartCoroutine(WaitTrigger());
        }

        private IEnumerator WaitTrigger()
        {
            while (!isActive)
            {
                yield return new WaitForSeconds(0.5f);
                print($"triggered: {_trigger.IsTriggered}");
                if (_trigger.IsTriggered)
                {
                    print("activate guil!!!");
                    isActive = true;
                    this.StopAndStartCoroutine(ref routine, GuillotineCycle());
                }
            }
        }
        
        IEnumerator GuillotineCycle()
        {
            while (isActive)
            {
                // Go Down (Fast)
                print(" guil go down!!!");

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
            return isGoingDown;
        }

        public void ResetToInitialState()
        {
            if (routine is not null)
            {
                StopCoroutine(routine);
                routine = null;
            }
            
            currentTween?.Kill();
            isActive = false;
            transform.position = startingPos;
            StartCoroutine(WaitTrigger());
        }
    }
}

