using UnityEngine.Serialization;

namespace NPC.NpcActions
{
  using System.Collections;
using DG.Tweening;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC.NpcActions
{
    public abstract class ComplexMoveAction : NpcAction
    {
        [SerializeField] protected float speed;
        [SerializeField] protected float minDistanceToPlayer;
        [SerializeField] protected LayerMask groundMask;
        [SerializeField] protected float groundCheckDistance;
        [SerializeField] protected int rayCount;
        [SerializeField] protected float raySpreadAngle;
        [SerializeField] protected float rayLength;
        [SerializeField] protected float wallCheckDistance;

        protected Coroutine walkRoutine;

        protected Vector2 GetMoveDirection(Npc npc)
        {
            return CoreManager.Instance.Player.transform.position.x > npc.transform.position.x
                ? Vector2.right
                : Vector2.left;
        }

        protected bool IsGroundAhead(Npc npc)
        {
            Vector2 origin = (GetMoveDirection(npc) == Vector2.right ? new Vector3(1.5f, 0, 0) : new Vector3(-1.5f, 0, 0))
                             + npc.transform.position + Vector3.down * 0.1f;
            Vector2 direction = Vector2.down;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, groundCheckDistance, groundMask);
            return hit.collider is not null;
        }

        protected float GetWallAheadHeight(Npc npc)
        {
            Vector2 origin = GetMoveDirection(npc) + (Vector2)npc.transform.position + Vector2.up;
            Vector2 direction = GetMoveDirection(npc);
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, groundMask);
            if (hit.collider is not null)
            {
                return hit.collider.bounds.max.y - npc.transform.position.y;
            }

            return 0;
        }

        protected Vector3? FindNextPlatform(Npc npc)
        {
            Vector2 characterDir = GetMoveDirection(npc);
            Vector2 origin = npc.transform.position + (GetMoveDirection(npc) == Vector2.right
                ? new Vector3(2.5f, 3, 0)
                : new Vector3(-2.5f, 3, 0));
            float startAngle = -raySpreadAngle / 2f;

            Vector3? closestPoint = null;
            float closestDist = Mathf.Infinity;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = startAngle + (raySpreadAngle / (rayCount - 1)) * i;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * GetMoveDirection(npc);

                RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, groundMask);
                RaycastHit2D hitDown = Physics2D.Raycast(origin, Vector2.down, 1, groundMask); // make sure the next platform is not the one we are standing on.
                Debug.DrawRay(origin, dir * rayLength, hit.collider != null ? Color.green : Color.red, 1f);

                if (hit.collider is not null && hit.collider != hitDown.collider)
                {
                    float dist = Vector2.Distance(origin, hit.point);
                    if (dist < closestDist)
                    {
                        Collider2D hitCol = hit.collider;
                        closestPoint = characterDir == Vector2.right ? new Vector3(
                            hitCol.bounds.min.x + Random.Range(1.2f, 2.2f),
                            hitCol.bounds.max.y, 0) : new Vector3(
                            hitCol.bounds.max.x - Random.Range(1.2f, 2.2f),
                            hitCol.bounds.max.y, 0);
                        closestDist = dist;
                    }
                }
            }

            return closestPoint;
        }

        protected void PerformJump(Npc npc, Vector3 target)
        {
            JumpMoveAction jump = new JumpMoveAction(2, target - npc.transform.position, 1f);
            npc.InterruptWithAction(jump);
        }

        protected void PerformWalk(Npc npc, Vector2 direction, float speed)
        {
            StopWalking();
            walkRoutine = CoreManager.Instance.Runner.StartCoroutine(WalkRoutine(npc, direction.normalized, speed));
        }

        protected void StopWalking()
        {
            if (walkRoutine != null)
            {
                CoreManager.Instance.Runner.StopCoroutine(walkRoutine);
                walkRoutine = null;
            }
        }

        private IEnumerator WalkRoutine(Npc npc, Vector2 direction, float speed)
        {
            while (true)
            {
                npc.transform.position += (Vector3)(direction * (speed * Time.deltaTime));
                yield return null;
            }
        }
        
        private void PerformJumpCancelDashUp(Npc npc)
        {
            JumpCancelDashUpAction jumpDash = new JumpCancelDashUpAction();
            npc.InterruptWithAction(jumpDash);
        }

        protected void PerformSpecialMovementIfNecessary(Npc npc)
        {
            if (!IsGroundAhead(npc))
            {
                StopWalking();
                var landingSpot = FindNextPlatform(npc);
                if (landingSpot.HasValue)
                    PerformJump(npc, landingSpot.Value);
            }

            float wallAheadHeight = GetWallAheadHeight(npc);
            if (wallAheadHeight > npc.MaxJumpHeight)
            {
                PerformJumpCancelDashUp(npc);
            }
            else if(wallAheadHeight > 0)
            {
                PerformJump(npc,npc.transform.position + new Vector3(wallCheckDistance + Random.Range(0.5f,1f),wallAheadHeight,0));
            }
        }
    }
}

}