using System;
using System.Collections;
using DG.Tweening;
using Managers;
using NPC.NpcActions.NPC.NpcActions;
using Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC.NpcActions
{
    [Serializable]
    public class FollowAction : ComplexMoveAction
    {
        [SerializeField] private Trigger stopFollowTrigger;

        private bool isFollowing;
        private Coroutine followCoroutine;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            isFollowing = true;
            npc.SetState(NpcState.Following);
            followCoroutine = CoreManager.Instance.Runner.StartCoroutine(FollowRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
            if (stopFollowTrigger != null && stopFollowTrigger.IsTriggered)
            {
                StopWalking();
                isFollowing = false;
                isCompleted = true;
            }
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            isFollowing = false;
            if (followCoroutine != null)
                CoreManager.Instance.Runner.StopCoroutine(followCoroutine);
        }

        private IEnumerator FollowRoutine(Npc npc)
        {
            PerformWalk(npc, GetMoveDirection(npc), npc.Speed);

            while (isFollowing)
            {
                float distanceToPlayer =
                    Vector2.Distance(npc.transform.position, CoreManager.Instance.Player.transform.position);

                if (distanceToPlayer <= minDistanceToPlayer)
                {
                    StopWalking();
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    if (walkRoutine is null)
                    {
                        PerformWalk(npc, GetMoveDirection(npc), npc.Speed);
                        yield return new WaitForSeconds(Random.Range(0.4f, 1f));
                    }
                }

                PerformSpecialMovementIfNecessary(npc);

                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}