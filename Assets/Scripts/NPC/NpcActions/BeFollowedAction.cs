using System;
using Managers;
using NPC.NpcActions.NPC.NpcActions;
using Triggers;

namespace NPC.NpcActions
{
    using System.Collections;
    using UnityEngine;

    [Serializable]
    public class BeFollowedAction : ComplexMoveAction
    {
        [SerializeField] private Trigger stopFollowTrigger;
        private Coroutine beFollowedCoroutine;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.IsFollowed = true;
            
            // npc.transform.GetComponent<Collider2D>().isTrigger = false;
            beFollowedCoroutine = CoreManager.Instance.Runner.StartCoroutine(BeFollowedRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
            base.UpdateAction(npc);
            if (stopFollowTrigger.IsTriggered)
            {
                Debug.Log("STOP BEING FOLLOWED");
                npc.IsFollowed = false;
                isCompleted = true;
                StopWalking(npc);
            }
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            npc.IsFollowed = false;
            // npc.transform.GetComponent<Collider2D>().isTrigger = true;
            StopWalking(npc);

            if (beFollowedCoroutine != null)
                CoreManager.Instance.Runner.StopCoroutine(beFollowedCoroutine);
        }

        private IEnumerator BeFollowedRoutine(Npc npc)
        {
            PerformWalk(npc, Vector2.right, npc.Speed);
            while (npc.IsFollowed)
            {
                float dist = npc.transform.position.x - CoreManager.Instance.Player.transform.position.x;
                if (dist < minDistanceToPlayer && waitingForPlayer)
                {
                    PerformWalk(npc, Vector2.right, npc.Speed);
                    waitingForPlayer = false;
                }
                else if( dist > minDistanceToPlayer)
                {
                    StopWalking(npc);
                    waitingForPlayer = true;
                }
                PerformSpecialMovementIfNecessary(npc);

                yield return new WaitForSeconds(0.2f);
            }

        }
    }
}