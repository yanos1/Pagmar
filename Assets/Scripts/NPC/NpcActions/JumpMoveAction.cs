using System;
using DG.Tweening;
using UnityEngine;

namespace NPC.NpcActions
{

    [Serializable]
    public class JumpMoveAction : MoveAction
    {
        [SerializeField] private float jumpPower;
        [SerializeField] int numJumps;

        public JumpMoveAction()
        {
            this.jumpPower = jumpPower;
            this.numJumps = numJumps;
        }
        public JumpMoveAction(float jumpPower, int numJumps = 1)
        {
            this.jumpPower = jumpPower;
            this.numJumps = numJumps;
        }

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