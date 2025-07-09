using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using UnityEngine;

namespace NPC.NpcActions
{

    [Serializable]
    public class JumpMoveAction : MoveAction
    {
        [SerializeField] private float jumpPower;
        [SerializeField] int numJumps;

        private BigSpine _spine;

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
            _spine = npc.GetComponent<BigSpine>();

            base.StartAction(npc);
            PerformMovement(npc);
            npc.SetState(NpcState.Jumping);
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
        }

        protected override void PerformMovement(Npc npc)
        {
            Debug.Log("[PerformMovement] Starting movement.");

            // 1. Play Jump animation
            string jumpAnim = _spine.GetAnimName(BigSpine.SpineAnim.Jump);
            Debug.Log($"[PerformMovement] Playing Jump animation: {jumpAnim}");
            _spine.PlayAnimation(jumpAnim, loop: false, fallbackAnimation: null, force: true);

            // 2. Start jump tween immediately
            Vector3 targetWorldPosition = npc.transform.position + targetPosition;
            Debug.Log($"[PerformMovement] Starting DOJump to: {targetWorldPosition}, Power: {jumpPower}, NumJumps: {numJumps}, Duration: {duration}");

            npc.transform.DOJump(targetWorldPosition, jumpPower, numJumps, duration)
                .SetEase(easeType)
                .OnStart(() =>
                {
                    // 3. Wait a short moment, then switch to JumpAir while in motion
                    CoreManager.Instance.Runner.StartCoroutine(SwitchToJumpAirAfterDelay(0.5f)); // adjust delay as needed
                })
                .OnComplete(() =>
                {
                    // 4. On landing
                    string landAnim = _spine.GetAnimName(BigSpine.SpineAnim.JumpLand);
                    string fallbackAnim = _spine.GetAnimName(BigSpine.SpineAnim.Walk);

                    Debug.Log($"[PerformMovement] DOJump complete. Playing JumpLand animation: {landAnim} with fallback: {fallbackAnim}");

                    _spine.PlayAnimation(
                        landAnim,
                        loop: false,
                        fallbackAnimation: fallbackAnim,
                        force: true,
                        onComplete: () =>
                        {
                            isCompleted = true;
                            Debug.Log("[PerformMovement] Movement completed.");
                        });

                });
        }

        private IEnumerator SwitchToJumpAirAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            string jumpAirAnim = _spine.GetAnimName(BigSpine.SpineAnim.JumpAir);
            Debug.Log($"[PerformMovement] Switching to JumpAir animation: {jumpAirAnim}");

            _spine.PlayAnimation(
                jumpAirAnim,
                loop: true,
                fallbackAnimation: null,
                force: true
            );
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