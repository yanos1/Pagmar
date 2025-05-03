using System;
using DG.Tweening;
using Enemies;
using Interfaces;
using Player;
using Terrain;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class ChargeAction : MoveAction
    {
        [SerializeField] private float obstacleCheckDistance = 1f;

        private Tween chargeTween;
        private bool jumpInserted = false;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Charging);
            Transform target = FindClosestTarget(npc);

            if (target == null)
            {
                Debug.LogWarning("No valid charge target found.");
                isCompleted = true;
                return;
            }

            Vector3 directionToTarget = target.position - npc.transform.position;
            targetPosition = new Vector3(directionToTarget.x, 0f, 0f); // Only horizontal charge
            PerformMovement(npc);

        }

        public override void UpdateAction(Npc npc)
        {
            if (npc == null || npc.transform == null)
                return;

            // Find the closest target (enemy or breakable object)
            Transform closestTarget = FindClosestTarget(npc);

            if (closestTarget != null)
            {
                float distanceToTarget = Vector2.Distance(npc.transform.position, closestTarget.position);

                // Check if the enemy is within a certain distance and stop charging if necessary
                if (distanceToTarget < obstacleCheckDistance)
                {
                    Debug.Log("Stopping charge due to nearby enemy.");
                    isCompleted = true;
                }
            }
        }

        protected override void PerformMovement(Npc npc)
        {
            int chargeDir = targetPosition.x > 0 ? -1 : 1;
            Vector3 targetRotation = new Vector3(0f, 0f, chargeDir * 30f);

            Sequence chargeSequence = DOTween.Sequence();

            chargeSequence.Append(npc.transform.DORotate(targetRotation, 0.2f));

            chargeSequence.Append(npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType));

            chargeSequence.Append(npc.transform.DORotate(Vector3.zero, 0.2f));

            chargeSequence.OnComplete(() => isCompleted = true);
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
            if (chargeTween != null && chargeTween.IsActive())
                chargeTween.Kill();

            npc.transform.DORotate(Vector3.zero, 0.2f);
            Debug.Log("go idle after charge.");
            isCompleted = false;
            npc.SetState(NpcState.Idle);
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
            Vector2 origin = npcTransform.position + Vector3.up * 0.1f;
            float maxDistance = 20f; // Can adjust based on charge vision
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;

            // Directions to check
            Vector2[] directions = { Vector2.right , Vector2.left };

            foreach (Vector2 dir in directions)
            {
                // Draw ray for 1 second
                Debug.DrawRay(origin, dir * maxDistance, Color.red, 1f);

                RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, maxDistance);

                foreach (RaycastHit2D hit in hits)
                {
                    Debug.Log($"hit {hit.transform.gameObject.name}");
                    if (hit.collider == null) continue;

                    var target = hit.collider.GetComponent<MonoBehaviour>();
                    if (target == null || target.transform == npcTransform) continue;

                    bool isBreakable = target is IBreakable;
                    bool isEnemy = target.GetType().IsSubclassOf(typeof(Enemy));

                    if (isBreakable || isEnemy)
                    {
                        float distance = Vector2.Distance(origin, hit.point);

                        if (distance < closestDistance)
                        {
                            Debug.Log($"closest target is {hit.collider.gameObject.name} with distance {distance}");
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
