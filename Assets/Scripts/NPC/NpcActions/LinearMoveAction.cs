using System;
using Camera;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace NPC.NpcActions
{
    [Serializable]
    public class LinearMoveAction : MoveAction
    {
        // [SerializeField] private EventNames callbackEvent;
        // [SerializeField] private bool useCallback;
        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            PerformMovement(npc);
        }

        protected override void PerformMovement(Npc npc)
        {
            Debug.Log($"moving npc in {duration} seconds");
            npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType)
                .OnComplete(
                    () =>
                    {
                        isCompleted = true;
                        Debug.Log("Completed Movement!");
                    });

        }

        public override void UpdateAction(Npc npc)
        {
        }
    }


}