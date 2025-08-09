using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using Managers;
using MoreMountains.Feedbacks;
using NPC.BigFriend;
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

            string jumpAnim = _spine.GetAnimName(BigSpine.SpineAnim.Jump);
            npc.Sounds.PlayJump(npc.transform);
            _spine.PlayAnimation(jumpAnim, loop: false, fallbackAnimation: null, force: true);

            Vector3 targetWorldPosition = npc.transform.position + targetPosition;

            npc.transform.DOJump(targetWorldPosition, jumpPower, numJumps, duration)
                .SetEase(easeType)
                .OnStart(() =>
                {
                    CoreManager.Instance.Runner.StartCoroutine(SwitchAnimationsOverTime());
                })
                .OnComplete(() =>
                {
                    // Ensures cleanup if needed
                    npc.Sounds.PlayLand(npc.transform);

                    npc.GetComponent<BigActions>().PlayLandFeedbacks();
                    isCompleted = true;
                });
        }

        private IEnumerator SwitchAnimationsOverTime()
        {
            float t1 = duration * 0.33f;
            float t2 = duration * 0.6f;

            yield return new WaitForSeconds(t1);
            string jumpAirAnim = _spine.GetAnimName(BigSpine.SpineAnim.JumpAir);
            _spine.PlayAnimation(jumpAirAnim, loop: true, fallbackAnimation: null, force: true);

            yield return new WaitForSeconds(t2 - t1);
            string landAnim = _spine.GetAnimName(BigSpine.SpineAnim.JumpLand);
            string fallbackAnim = _spine.GetAnimName(BigSpine.SpineAnim.Walk);

            _spine.PlayAnimation(
                landAnim,
                loop: false,
                fallbackAnimation: fallbackAnim,
                force: true,
                onComplete: () =>
                {
                    Debug.Log("[PerformMovement] JumpLand animation completed.");
                });
        }

        public override void UpdateAction(Npc npc)
        {
            return;
        }
    }
}
