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
        private bool isRunning;

        public override void StartAction(Npc npc)
        {
            isRunning = true;
            npc.transform.GetComponent<Collider2D>().isTrigger = false;
            beFollowedCoroutine = CoreManager.Instance.Runner.StartCoroutine(BeFollowedRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
            if (stopFollowTrigger.IsTriggered)
            {
                isRunning = false;
                isCompleted = true;
                StopWalking();
            }
        }

        public override void ResetAction(Npc npc)
        {
            isRunning = false;
            npc.transform.GetComponent<Collider2D>().isTrigger = true;
            StopWalking();

            if (beFollowedCoroutine != null)
                CoreManager.Instance.Runner.StopCoroutine(beFollowedCoroutine);
        }

        private IEnumerator BeFollowedRoutine(Npc npc)
        {
            while (isRunning)
            {
                float dist = npc.transform.position.x - CoreManager.Instance.Player.transform.position.x;
                if (dist < minDistanceToPlayer)
                {
                    PerformWalk(npc, Vector2.right, speed);
                }
                else
                {
                    StopWalking();
                }
                PerformSpecialMovementIfNecessary(npc);

                yield return new WaitForSeconds(0.2f);
            }

        }
    }
}