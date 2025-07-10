using Managers;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace NPC.NpcActions
{
    [System.Serializable]
    public class JumpCancelDashUpAction : NpcAction
    {
        [SerializeField] private float jumpDuration;
        [SerializeField] private float dashDuration;
        [SerializeField] private Vector3 jumpTarget;
        [SerializeField] private int jumpPower;
        [SerializeField] private Vector3 dashTarget;

        private Tween jumpTween;
        private Tween dashTween;

        private BigSpine _spine;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            _spine = npc.GetComponent<BigSpine>();
            isCompleted = false;
            CoreManager.Instance.Runner.StartCoroutine(JumpDashRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            jumpTween?.Kill();
            dashTween?.Kill();
        }

        private IEnumerator JumpDashRoutine(Npc npc)
        {
            npc.SetState(NpcState.Jumping);

            // Start Jump Anim
            string jumpAnim = _spine.GetAnimName(BigSpine.SpineAnim.Jump);
            _spine.PlayAnimation(jumpAnim, loop: false, fallbackAnimation: null, force: true);

            // Start animation switch routine
            CoreManager.Instance.Runner.StartCoroutine(SwitchAnimationsOverTime(jumpDuration));

            // Jump movement
            Vector3 jumpT = npc.transform.position + jumpTarget;
            jumpTween = npc.transform.DOJump(jumpT, jumpPower, 1, jumpDuration).SetEase(Ease.OutQuad);

            // Cancel mid-air
            yield return new WaitForSeconds(jumpDuration / 2f);
            if (jumpTween != null && jumpTween.IsActive())
            {
                jumpTween.Kill();
                Debug.Log("[JumpCancel] Jump tween cancelled mid-air.");
            }

            // Dash upward
            npc.SetState(NpcState.Charging);
            string dash = _spine.GetAnimName(BigSpine.SpineAnim.Dash);
            _spine.PlayAnimation(dash, loop: false, fallbackAnimation: null, force: true);   
            Vector3 dashT = npc.transform.position + dashTarget;
            dashTween = npc.transform.DOMove(dashT, dashDuration).SetEase(Ease.OutExpo);

            yield return dashTween.WaitForCompletion();

            isCompleted = true;
            Debug.Log("[JumpCancel] Dash complete. Action finished.");
        }

        private IEnumerator SwitchAnimationsOverTime(float totalDuration)
        {
            float t1 = totalDuration * 0.33f;
            yield return new WaitForSeconds(t1);
            string jumpAirAnim = _spine.GetAnimName(BigSpine.SpineAnim.JumpAir);
            _spine.PlayAnimation(jumpAirAnim, loop: true, fallbackAnimation: null, force: true);
            Debug.Log($"[JumpCancel] Switched to JumpAir at {t1}s");
        }
    }
}
