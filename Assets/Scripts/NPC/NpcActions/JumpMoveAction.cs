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
        public JumpMoveAction(float jumpPower, Vector3 targetPosition, float duration, int numJumps = 1)
        {
            this.jumpPower = jumpPower;
            this.targetPosition = targetPosition;
            this.numJumps = numJumps;
            this.duration = duration;
        }

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            PerformMovement(npc);
            npc.SetState(NpcState.Jumping);
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            npc.SetState(NpcState.Idle);
        }

        protected override void PerformMovement(Npc npc)
        {
            npc.transform.DOJump(npc.transform.position + targetPosition, jumpPower, numJumps, duration)
                .SetEase(easeType)
                .OnComplete(() => isCompleted = true);
        }

        public override void UpdateAction(Npc npc)
        {
            return;
        }
    }

}