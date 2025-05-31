using Managers;


using System.Collections;
using DG.Tweening;
using NPC.NpcActions;
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
            Vector3 jumpT = npc.transform.position + jumpTarget;

            // Start jump
            jumpTween = npc.transform.DOJump(jumpT,jumpPower,1,jumpDuration).SetEase(Ease.OutQuad);

            // Wait for cancel time
            yield return new WaitForSeconds(jumpDuration / 2);

            // Cancel jump
            if (jumpTween != null && jumpTween.IsActive())
                jumpTween.Kill();

            yield return null; // safety

            // Dash upward
            Vector3 dashT = npc.transform.position + dashTarget;
            dashTween = npc.transform.DOMove(dashT, dashDuration).SetEase(Ease.OutExpo);

            yield return dashTween.WaitForCompletion();
            isCompleted = true;

        }
    }
}

