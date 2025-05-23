using System.Collections;
using DG.Tweening;
using Interfaces;
using Player;
using SpongeScene;
using Triggers;
using UnityEngine;

namespace Obstacles
{
    public enum MoveDirection
    {
        X,
        Y
    }

    public class GuillotineTrap : MonoBehaviour, IKillPlayer, IResettable,IBreakable
    {
        [Header("Movement Settings")]
        [SerializeField] private MoveDirection moveDirection = MoveDirection.Y;
        [SerializeField] private float moveDistance = 5f;
        [SerializeField] private float downDuration = 0.2f;
        [SerializeField] private float upDuration = 1.5f;
        [SerializeField] private float delayBetweenCycles = 1f;

        [Header("Trigger")]
        [SerializeField] private Trigger _trigger;

        private bool isActive = false;
        private bool isGoingDown = false;
        private Vector3 startingPos;

        private Coroutine moveRoutine;
        private Coroutine waitRoutine;
        private Tween currentTween;

        private void Start()
        {
            startingPos = transform.position;
            waitRoutine = StartCoroutine(WaitForTrigger());
        }

        private IEnumerator WaitForTrigger()
        {
            while (!isActive)
            {
                yield return new WaitForSeconds(0.5f);
                if (_trigger.IsTriggered)
                {
                    isActive = true;
                    this.StopAndStartCoroutine(ref moveRoutine, GuillotineCycle());
                }
            }
        }

        private IEnumerator GuillotineCycle()
        {
            while (isActive)
            {
                Vector3 downPos = moveDirection == MoveDirection.Y
                    ? new Vector3(transform.position.x, startingPos.y - moveDistance, transform.position.z)
                    : new Vector3(startingPos.x - moveDistance, transform.position.y, transform.position.z);

                Vector3 upPos = startingPos;

                isGoingDown = true;
                currentTween = transform.DOMove(downPos, downDuration)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => isGoingDown = false);

                yield return currentTween.WaitForCompletion();
                yield return new WaitForSeconds(delayBetweenCycles);

                currentTween = transform.DOMove(upPos, upDuration)
                    .SetEase(Ease.OutQuad);

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
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }

            currentTween?.Kill();
            isActive = false;
            isGoingDown = false;
            transform.position = startingPos;

            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
            }

            waitRoutine = StartCoroutine(WaitForTrigger());
        }

        public void OnBreak()
        {
            throw new System.NotImplementedException();
        }

        public void OnHit(Vector2 hitDir, PlayerManager.PlayerStage stage)
        {
            throw new System.NotImplementedException();
        }
    }
}
