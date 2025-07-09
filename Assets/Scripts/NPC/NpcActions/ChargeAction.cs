using System;
using DG.Tweening;
using Enemies;
using FMODUnity;
using Interfaces;
using NPC.BigFriend;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace NPC.NpcActions
{
    [Serializable]
    public class ChargeAction : MoveAction
    {
        [SerializeField] private float obstacleCheckDistance = 1f;
        private BigSpine _spine;

        private Tween chargeTween;
        private bool jumpInserted = false;
        private float targetOffset;
        private Transform target;
        private bool isChargingAnimPlaying = false;
        [SerializeField] private EventReference hitSound;

        public override void StartAction(Npc npc)
        {
            _spine = npc.GetComponent<BigSpine>();
            base.StartAction(npc);
            npc.SetState(NpcState.Charging);
            target = FindClosestTarget(npc);

            if (target == null)
            {
                Debug.LogWarning("No valid charge target found.");
                isCompleted = true;
                return;
            }

            targetOffset = target.position.x - npc.transform.position.x;
            npc.TurnAround(new Vector2(targetOffset, 0));

            string runAnim = _spine.GetAnimName(BigSpine.SpineAnim.Run);
            Debug.Log($"[ChargeAction] Playing Run Animation: {runAnim}");
            _spine.PlayAnimation(runAnim, loop: true, fallbackAnimation: null, force: true);

            PerformMovement(npc);
        }

        public override void UpdateAction(Npc npc)
        {
            if (target == null || isChargingAnimPlaying || isCompleted)
                return;

            float distToTarget = Vector2.Distance(npc.transform.position, target.position);
            if (distToTarget < 10f)
            {
                string chargeAnim = _spine.GetAnimName(BigSpine.SpineAnim.Dash);
                Debug.Log($"[ChargeAction] Distance to target < 5. Switching to Charge Animation: {chargeAnim}");
                _spine.PlayAnimation(chargeAnim, loop: true, fallbackAnimation: null, force: true);
                isChargingAnimPlaying = true;
            }
        }

        protected override void PerformMovement(Npc npc)
        {
            int chargeDir = targetOffset > 0 ? -1 : 1;

            Sequence chargeSequence = DOTween.Sequence();
            chargeSequence.Append(npc.transform.DOMove(npc.transform.position + Vector3.right * (targetOffset + chargeDir), duration)
                .SetEase(easeType));

            chargeSequence.OnComplete(() =>
            {
                isCompleted = true;
                npc.GetComponent<BigActions>().DoSmileAnim(5);
                Debug.Log("[ChargeAction] Charge complete.");
            });
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            if (chargeTween != null && chargeTween.IsActive())
                chargeTween.Kill();
        }

        private bool IsObstacleInFront(Npc npc)
        {
            Vector2 origin = npc.transform.position;
            Vector2 direction = targetPosition.normalized;
            Vector2 boxSize = new Vector2(1f, 1f);
            float angle = 0f;

            RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, angle, direction, obstacleCheckDistance);

            if (hit.collider != null)
            {
                return hit.collider.GetComponent<PlayerMovement>() is not null;
            }

            return false;
        }

        private Transform FindClosestTarget(Npc npc)
        {
            Transform npcTransform = npc.transform;
            Vector2 origin = npcTransform.position + Vector3.up;
            float maxDistance = 20f;
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;

            Vector2[] directions = { Vector2.right, Vector2.left };

            foreach (Vector2 dir in directions)
            {
                Debug.DrawRay(origin, dir * maxDistance, Color.red, 1f);

                RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, maxDistance);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider == null) continue;

                    var breakable = hit.collider.gameObject.GetComponent<MonoBehaviour>();
                    bool isEnemy = hit.collider.GetComponent<Enemy>() != null;
                    bool isBreakable = breakable is IBreakable;

                    if (isBreakable || isEnemy)
                    {
                        float distance = Vector2.Distance(origin, hit.point);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTarget = hit.collider.transform;
                        }
                    }
                }
            }

            return closestTarget;
        }
    }
}