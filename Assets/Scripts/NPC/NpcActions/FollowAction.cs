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
            CoreManager.Instance.Player.SetFollowedBy(npc);
            npc.SetState(NpcState.Following);
            followCoroutine = CoreManager.Instance.Runner.StartCoroutine(FollowRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
            if (stopFollowTrigger is not null && stopFollowTrigger.IsTriggered)
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
            CoreManager.Instance.Player.SetFollowedBy(null);
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
                        yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
                        PerformWalk(npc, GetMoveDirection(npc), npc.Speed);
                    }
                }

                PerformSpecialMovementIfNecessary(npc);
                if (IsCompleted) yield break;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}