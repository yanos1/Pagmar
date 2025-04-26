using Managers;

namespace NPC.NpcActions
{
  using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace NPC.NpcActions
{
    public class JumpCancelDashUpAction : NpcAction
    {
        [SerializeField] private float jumpHeight = 2f;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private float cancelTime = 0.25f;

        [SerializeField] private float dashHeight = 3f;
        [SerializeField] private float dashDuration = 0.7f;
        
        private Tween jumpTween;
        private Tween dashTween;

        public JumpCancelDashUpAction(){}
      
        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            isCompleted = false;
            CoreManager.Instance.Runner.StartCoroutine(JumpDashRoutine(npc));
        }

        public override void UpdateAction(Npc npc)
        {
            return;
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            
            // if (jumpCancelDashRoutine != null)
            // {
            //     CoreManager.Instance.Runner.StopCoroutine(jumpCancelDashRoutine);
            //     jumpCancelDashRoutine = null;
            // }
            //
            // jumpTween?.Kill();
            // dashTween?.Kill();
            
        }

        private IEnumerator JumpDashRoutine(Npc npc)
        {
            Vector3 startPos = npc.transform.position;
            Vector3 jumpTarget = startPos + new Vector3(1.5f,npc.MaxJumpHeight);

            // Start jump
            jumpTween = npc.transform.DOJump(npc.transform.position + new Vector3(0.7f,0.7f,0),2,1,jumpDuration).SetEase(Ease.OutQuad);

            // Wait for cancel time
            yield return new WaitForSeconds(npc.JumpDuration / 2);

            // Cancel jump
            if (jumpTween != null && jumpTween.IsActive())
                jumpTween.Kill();

            yield return null; // safety

            // Dash upward
            Vector3 dashTarget = npc.transform.position + new Vector3(1,1) * npc.DashDistance;
            dashTween = npc.transform.DOMove(dashTarget, dashDuration).SetEase(Ease.OutExpo);

            yield return dashTween.WaitForCompletion();
            isCompleted = true;

        }
    }
}

}