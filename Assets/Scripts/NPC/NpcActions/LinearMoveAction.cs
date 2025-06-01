using System;
using System.Collections;
using Camera;
using DG.Tweening;
using Managers;
using MoreMountains.Feedbacks;
using ScripableObjects;
using ScriptableObjects;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class LinearMoveAction : MoveAction
    {
        [SerializeField] private PlayerSounds sounds;
        [SerializeField] private MMF_Player stepFeedbacks;

        private Coroutine moveCoroutine;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Walking);
            PerformMovement(npc);
        }

        protected override void PerformMovement(Npc npc)
        {
            Debug.Log($"[Coroutine] Moving NPC over {duration} seconds");

            if (sounds != null)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.walkSound, npc.transform.position);
            }

            stepFeedbacks?.PlayFeedbacks();

            // Stop any previous movement coroutine (just in case)
            if (moveCoroutine != null)
            {
                npc.StopCoroutine(moveCoroutine);
            }

            moveCoroutine = npc.StartCoroutine(MoveOverTime(npc));
        }

        private IEnumerator MoveOverTime(Npc npc)
        {
            Vector3 startPos = npc.transform.position;
            Vector3 endPos = startPos + targetPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                yield return new WaitForFixedUpdate(); // Similar to LateUpdate

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float easedT = DOVirtual.EasedValue(0f, 1f, t, easeType);

                npc.transform.position = Vector3.LerpUnclamped(startPos, endPos, easedT);
            }

            npc.transform.position = endPos;
            isCompleted = true;
            Debug.Log("Completed Coroutine Movement!");
        }

        public override void UpdateAction(Npc npc)
        {
            // No update logic needed here for coroutine-based movement
        }
    }
}
