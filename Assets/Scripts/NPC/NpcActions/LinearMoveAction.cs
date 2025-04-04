using System;
using DG.Tweening;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class LinearMoreAction : MoveAction
    {
        protected override void PerformMovement(Npc npc)
        {
            npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType)
                .OnComplete(() => isCompleted = true);
        }

        public override void UpdateAction(Npc npc)
        {
        }
    }


}