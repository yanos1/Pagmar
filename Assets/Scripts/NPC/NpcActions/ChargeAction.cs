using System;
using DG.Tweening;
using Player;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class ChargeAction : MoveAction
    {
        [SerializeField] private float obstacleCheckDistance = 1f;
        
        private Tween chargeTween;

        private bool jumpInserted = false;

        public override void UpdateAction(Npc npc)
        {
            if (isCompleted || jumpInserted) return;

            if (IsObstacleInFront(npc))
            {
                // Tell the NPC to insert a jump, then come back to this charge action
                // npc.AddAction(new JumpMoveAction(5,,5));
                jumpInserted = true;
                isCompleted = true; // Mark this as done so it doesn’t keep checking
                return;
            }

            // No obstacle, begin movement
            PerformMovement(npc);
            jumpInserted = true; // Avoid repeated movement
        }

        protected override void PerformMovement(Npc npc)
        {
            Sequence chargeSequence = DOTween.Sequence();

            chargeSequence.Append(npc.transform.DORotate(new Vector3(0f, 0f, -30f), 0.2f));

            chargeSequence.Append(npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType));

            chargeSequence.Append(npc.transform.DORotate(Vector3.zero, 0.2f));

            chargeSequence.OnComplete(() => isCompleted = true);
        }

        public override void ResetAction(Npc npc)
        {
            if (chargeTween != null && chargeTween.IsActive())
                chargeTween.Kill();

            // If the sequence is active, kill it and reset the rotation
            npc.transform.DORotate(Vector3.zero, 0.2f); // Reset rotation back to zero

            isCompleted = false;
        }
        private bool IsObstacleInFront(Npc npc)
        {
            Vector2 origin = npc.transform.position;
            Vector2 direction = targetPosition.normalized;

            // Define your box size — you can tweak this as needed
            Vector2 boxSize = new Vector2(1f, 1f); // width, height of the box
            float angle = 0f; // No rotation on the box

            RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, angle, direction, obstacleCheckDistance);

            if (hit.collider != null)
            {
                return hit.collider.GetComponent<PlayerMovement>() is not null;
            }

            return false;
        }
    }
}