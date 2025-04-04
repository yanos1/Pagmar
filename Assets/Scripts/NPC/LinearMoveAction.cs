using System;

namespace NPC
{
    using DG.Tweening;
    using UnityEngine;

    [Serializable]
    public class LinearMoreAction : MoveAction
    {
        protected override void PerformMovement(Npc npc)
        {
            Vector3 adjustedTargetPosition = new Vector3(targetPosition.x, npc.transform.position.y, targetPosition.z);
        
            npc.transform.DOMove(adjustedTargetPosition, duration)
                .SetEase(easeType)
                .OnComplete(() => isCompleted = true);
        }

        public override void UpdateAction(Npc npc)
        {
        }
    }


}