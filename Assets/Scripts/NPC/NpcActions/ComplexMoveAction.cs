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
            [SerializeField] private LayerMask groundMask;

            [SerializeField] protected float minDistanceToPlayer = 5;
            protected float groundCheckDistance = 0.3f;
            protected int rayCount = 15;
            protected float raySpreadAngle = 120;
            [SerializeField] protected float rayLength = 20;
            protected float wallCheckDistance = 4.5f;

            protected Coroutine walkRoutine;


            public override void StartAction(Npc npc)
            {
                base.StartAction(npc);
            }

            public override void UpdateAction(Npc npc)
            {
                if (npc.Rb.linearVelocity.y < -3) // npc is falling
                {
                    isCompleted = true; 
                }
            }

            protected Vector2 GetMoveDirection(Npc npc)
            {
                if (npc.State == NpcState.Followed) return Vector2.right;

                return CoreManager.Instance.Player.transform.position.x > npc.transform.position.x
                    ? Vector2.right
                    : Vector2.left;
            }

            protected bool IsGroundAhead(Npc npc, Vector2 dir)
            {
                Vector2 origin = (dir == Vector2.right
                                     ? new Vector3(1.5f, 0, 0)
                                     : new Vector3(-1.5f, 0, 0))
                                 + npc.transform.position;
                Vector2 direction = Vector2.down;

                // Draw the ray in the Scene view
                Debug.DrawRay(origin, direction * groundCheckDistance, Color.black);
                Debug.Log(groundMask);
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, groundCheckDistance, groundMask);
                return hit.collider is not null;
            }


            protected Collider2D GetWallAhead(Npc npc)
            {
                Vector2 origin = GetMoveDirection(npc) + (Vector2)npc.transform.position + Vector2.up;
                Vector2 direction = GetMoveDirection(npc);
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, groundMask);
                if (hit.collider is not null)
                {
                    return hit.collider;
                }

                return null;
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
                    RaycastHit2D
                        hitDown = Physics2D.Raycast(origin, Vector2.down, 1,
                            groundMask); // make sure the next platform is not the one we are standing on.
                    Debug.DrawRay(origin, dir * rayLength, hit.collider != null ? Color.green : Color.red, 1f);

                    if (hit.collider is not null && hit.collider != hitDown.collider)
                    {
                        float dist = Vector2.Distance(origin, hit.point);
                        if (dist < closestDist)
                        {
                            Collider2D hitCol = hit.collider;
                            closestPoint = characterDir == Vector2.right
                                ? new Vector3(
                                    hitCol.bounds.min.x + Random.Range(1.2f, 2.2f),
                                    hitCol.bounds.max.y, 0)
                                : new Vector3(
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
                direction = direction.normalized;

                while (true)
                {
                    Vector2 currentPos = npc.transform.position;
                    Vector2 nextPos = currentPos + direction * (speed * Time.deltaTime);

                    npc.transform.position = Vector2.MoveTowards(currentPos, nextPos, speed * Time.deltaTime);

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
                var currentDir = GetMoveDirection(npc);
                if (!IsGroundAhead(npc, currentDir))
                {
                    Debug.Log("44 No Ground Ahead");
                    
                    var landingSpot = FindNextPlatform(npc);
                    if (landingSpot.HasValue)
                    {
                        Debug.Log($"44 found landing spot: {landingSpot.Value.x} ");
                        if ((npc.State == NpcState.Followed) || 
                            (npc.State == NpcState.Following && IsPlayerBeyondLandingSpot(currentDir, landingSpot.Value.x)))

                        {
                            Debug.Log($"44 player is not interrupting jump with pos {CoreManager.Instance.Player.transform.position.x} ");

                            PerformJump(npc, landingSpot.Value);
                        }
                        else
                        {
                            Debug.Log($"44 player is interrupting jump with pos {CoreManager.Instance.Player.transform.position.x} stopping");

                            StopWalking();
                        }
                    }
                    else
                    {
                        StopWalking();
                        isCompleted = true;
                    }
                }

                Collider2D wallAhead = GetWallAhead(npc);
                if (wallAhead)
                {
                    float wallAheadHeight = wallAhead.bounds.max.y - npc.transform.position.y;

                    if (npc.State == NpcState.Followed || npc.State == NpcState.Following &&
                        CoreManager.Instance.Player.transform.position.y > wallAhead.bounds.max.y)
                        if (wallAheadHeight > npc.MaxJumpHeight)
                        {
                            PerformJumpCancelDashUp(npc);
                        }
                        else if (wallAheadHeight > 0)
                        {
                            PerformJump(npc,
                                npc.transform.position + new Vector3(wallCheckDistance + Random.Range(0.5f, 1f),
                                    wallAheadHeight, 0));
                        }
                }
            }
            
            private bool IsPlayerBeyondLandingSpot(Vector2 currentDir, float landingX)
            {
                float playerX = CoreManager.Instance.Player.transform.position.x;

                if (currentDir == Vector2.right)
                    return playerX > landingX;

                if (currentDir == Vector2.left)
                    return playerX < landingX;

                return false;
            }

        }
    }
}