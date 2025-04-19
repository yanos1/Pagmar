using System;
using System.Collections;
using System.Collections.Generic;
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
            Debug.Log($"target pos of jump: {targetPosition}");
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

        private IEnumerator WaitForGround(Npc npc, Rigidbody2D rb)
        {
            int groundLayer = LayerMask.GetMask("Ground");

            while (true)
            {
                // Raycast down to check if touching ground
                RaycastHit2D hit = Physics2D.Raycast(npc.transform.position, Vector2.down, 0.1f, groundLayer);

                if (hit.collider != null)
                {
                    // Hit the ground

                    rb.bodyType = RigidbodyType2D.Kinematic; // Optional: Stop physics after landing
                    yield break;
                }

                yield return null; // Wait a frame
            }
        
        }

        public override void UpdateAction(Npc npc)
        {
            return;
        }
    }

}