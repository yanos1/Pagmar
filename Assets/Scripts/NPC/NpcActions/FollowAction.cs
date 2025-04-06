using System;
using System.Collections;
using DG.Tweening;
using Managers;
using Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NPC.NpcActions
{
    [Serializable]
    public class FollowAction : NpcAction
    {
        [SerializeField] private Trigger stopFollowTrigger;
        [SerializeField] private float followSpeed = 2f;
        [SerializeField] private float minDistanceToPlayer = 1.5f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private int rayCount = 20;
        [SerializeField] private float raySpreadAngle = 60f;
        [SerializeField] private float rayLength = 20f;

        private bool isFollowing;
        private Coroutine followCoroutine;
        private Coroutine walkRoutine;


        public override void StartAction(Npc npc)
        {
            isFollowing = true;
            Vector2 dir = CoreManager.Instance.Player.transform.position.x > npc.transform.position.x
                ? Vector2.right
                : Vector2.left;
            followCoroutine = CoreManager.Instance.Runner.StartCoroutine(FollowRoutine(npc, dir));
        }

        public override void UpdateAction(Npc npc)
        {
            if (stopFollowTrigger != null && stopFollowTrigger.IsTriggered)
            {
                Debug.Log("Follow stopped!");
                isFollowing = false;
                isCompleted = true;
            }
        }

        public override void ResetAction(Npc npc)
        {
            isFollowing = false;
            if (followCoroutine != null)
                CoreManager.Instance.Runner.StopCoroutine(followCoroutine);
        }

        private IEnumerator FollowRoutine(Npc npc, Vector2 dir)
        {
            Walk(npc, dir, followSpeed);

            while (isFollowing)
            {
                float distanceToPlayer =
                    Vector2.Distance(npc.transform.position, CoreManager.Instance.Player.transform.position);

                // If close enough, don't move
                if (distanceToPlayer <= minDistanceToPlayer)
                {
                    StopWalking();
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    if (walkRoutine is null)
                    {
                        var newDir = GetDirection(npc);
                        yield return new WaitForSeconds(Random.Range(0.4f, 1f));
                        Walk(npc, newDir, followSpeed);
                    }
                }

                if (!IsGroundAhead(npc))
                {
                    StopWalking();
                    Vector3? landingSpot = FindNextPlatform(npc);

                    if (landingSpot != null)
                    {
                        PerformJump(npc, landingSpot.Value);
                    }
                    else
                    {
                        Debug.Log("No platform found to jump!");
                    }
                }

                yield return new WaitForSeconds(0.2f);
            }
        }

        private static Vector2 GetDirection(Npc npc)
        {
            Vector2 newDir = CoreManager.Instance.Player.transform.position.x > npc.transform.position.x
                ? Vector2.right
                : Vector2.left;
            return newDir;
        }


        private bool IsGroundAhead(Npc npc)
        {
            Vector2 origin = (GetDirection(npc) == Vector2.right ? new Vector3(1.5f,0,0) : new Vector3(-1.5f,0,0)) + npc.transform.position + Vector3.down * 0.1f;
            Vector2 direction = Vector2.down;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, groundCheckDistance, groundMask);
            return hit.collider is not null;
        }

        private Vector3? FindNextPlatform(Npc npc)
        {
            Vector2 origin = npc.transform.position + (GetDirection(npc) == Vector2.right ? new Vector3(2.5f,3,0) : new Vector3(-2.5f,-3,0));
            float startAngle = -raySpreadAngle / 2f;

            Vector3? closestPoint = null;
            float closestDist = Mathf.Infinity;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = startAngle + (raySpreadAngle / (rayCount - 1)) * i;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * GetDirection(npc);

                RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, groundMask);
                Debug.DrawRay(origin, dir * rayLength, hit.collider != null ? Color.green : Color.red, 1f);

                if (hit.collider is not null)
                {
                    float dist = Vector2.Distance(origin, hit.point);
                    if (dist < closestDist)
                    {
                        Collider2D hitCol = hit.collider.gameObject.GetComponent<Collider2D>();
                        closestPoint =
                            new Vector3(
                                hitCol.bounds.min.x + Random.Range(1.2f, 2.2f),
                                hitCol.bounds.max.y + 0.1f, 0);
                        closestDist = dist;
                    }
                }
            }

            return closestPoint;
        }

        private void PerformJump(Npc npc, Vector3 target)
        {
            JumpMoveAction jump = new JumpMoveAction(2, target-npc.transform.position,1f);
            npc.InterruptWithAction(jump);
        }

        public void Walk(Npc npc, Vector2 direction, float speed)
        {
            StopWalking(); // Stop existing walk if any
            walkRoutine = CoreManager.Instance.Runner.StartCoroutine(WalkRoutine(npc, direction.normalized, speed));
        }

        public void StopWalking()
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
    }
}