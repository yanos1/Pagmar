using System;
using DG.Tweening;
using UnityEngine;

namespace NPC
{

    [Serializable]
    public class JumpMoveAction : MoveAction
    {
        [SerializeField] private float jumpPower;
        [SerializeField] int numJumps;
        protected override void PerformMovement(Npc npc)
        {
            npc.transform.DOJump(targetPosition, jumpPower, numJumps, duration)
                .SetEase(easeType)
                .OnComplete(() => isCompleted = true);
        }

        public override void UpdateAction(Npc npc)
        {
            return;
        }
    }

}