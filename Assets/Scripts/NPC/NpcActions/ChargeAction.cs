using System;
using DG.Tweening;
using Enemies;
using Interfaces;
using NPC.BigFriend;
using Player;
using Terrain;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace NPC.NpcActions
{
    [Serializable]
    public class ChargeAction : MoveAction
    {
        [SerializeField] private float obstacleCheckDistance = 1f;

        private Tween chargeTween;
        private bool jumpInserted = false;
        private float targetOffset;
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

            targetOffset = target.position.x - npc.transform.position.x;
            npc.TurnAround(new Vector2(targetOffset, 0));
            PerformMovement(npc);

        }

        public override void UpdateAction(Npc npc)
        {
           
        }

        protected override void PerformMovement(Npc npc)
        {
            int chargeDir = targetOffset > 0 ? -1 : 1;

            Sequence chargeSequence = DOTween.Sequence();
            chargeSequence.Append(npc.transform.DOMove(npc.transform.position + Vector3.right*(targetOffset + chargeDir), duration)
                .SetEase(easeType));


            chargeSequence.OnComplete(() =>
            {
                
                isCompleted = true;
                npc.GetComponent<BigActions>().DoSmileAnim(5);
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

                    var breakable = hit.collider.gameObject.GetComponent<MonoBehaviour>();
                    bool isEnemy = hit.GetType().IsSubclassOf(typeof(Enemy));
                    bool isBreakbale = breakable is IBreakable;
                    
                    if (isBreakbale || isEnemy)
                    {
                        Debug.Log("target found 1!");
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
