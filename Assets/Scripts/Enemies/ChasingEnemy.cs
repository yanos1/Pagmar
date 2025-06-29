using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Managers;
using Spine.Unity;

public class ChasingEnemy : Rammer, IResettable
{
    [SerializeField] private Rammer player;
    private float contactRadius;
    private NavMeshAgent agent;
    private float rotationSpeed = 3f;
    private Collider2D col;
    private Vector3 startingPosition;
    private Vector3 playerResetPosition;
    private bool chase = false;
    private float positionHistoryDuration = 17f;
    private List<(float time, Vector3 position)> positionHistory = new List<(float, Vector3)>();

    // Caching
    private Vector3 lastProcessedPlayerResetPos = Vector3.positiveInfinity;
    private Vector3 cachedBestResetPosition = Vector3.zero;
    private bool hasCachedReset = false;


    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
        StopAllCoroutines();
    }

    private void OnReachedCheckpoint(object obj)
    {
        if (obj is Vector3 pos)
        {
            playerResetPosition = pos;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider2D>();
        contactRadius = Mathf.Abs(col.bounds.max.x - transform.position.x);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        CurrentForce = 5;
        startingPosition = transform.position;
    }

    void Update()
    {
        if (!chase) return;

        agent.SetDestination(player.transform.position);

        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        float distance = Vector2.Distance(enemyPos, playerPos);

        if (distance <= contactRadius)
        {
            // RammerManager.Instance.ResolveRam(player, this); (Handled by player now)
        }

        Vector3 velocity = agent.velocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = velocity.normalized;
            float targetAngle = MapYToZRotation(direction.y);
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private float MapYToZRotation(float y)
    {
        return Mathf.Lerp(0f, 180f, (y + 1f) / 2f);
    }

    public override void OnRam(Vector2 ramDirNegative, float againstForce)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public override void OnRammed(float fromForce)
    {
    }
    

    public override void ApplyKnockback(Vector2 direction, float force) { }

    public void ResetToInitialState()
    {
        if (!chase) return;

        // Reuse cached position if the reset point hasn't changed
        if (hasCachedReset && playerResetPosition == lastProcessedPlayerResetPos)
        {
            agent.Warp(cachedBestResetPosition);
            agent.isStopped = false;
            print($"[CACHED] chosen pos {cachedBestResetPosition}");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerResetPosition);

        var considerablePositions = new Dictionary<float, Vector3>();
        foreach (var (time, pos) in positionHistory)
        {
            float distance = Vector3.Distance(playerResetPosition, pos);
            if (pos.x < playerResetPosition.x && distance > 14.5f)
            {
                considerablePositions[distance] = pos;
            }
        }

        if (considerablePositions.Count > 0)
        {
            float closestDistance = -1f;
            float smallestDiff = float.MaxValue;

            foreach (var kvp in considerablePositions)
            {
                float diff = Mathf.Abs(kvp.Key - 18f);
                if (diff < smallestDiff)
                {
                    smallestDiff = diff;
                    closestDistance = kvp.Key;
                }
            }

            Vector3 bestPosition = considerablePositions[closestDistance];
            agent.Warp(bestPosition);
            agent.isStopped = false;
            print($"[CALCULATED] chosen pos {bestPosition}");

            // Cache result
            lastProcessedPlayerResetPos = playerResetPosition;
            cachedBestResetPosition = bestPosition;
            hasCachedReset = true;
        }
    }

    public void StartChase()
    {
        chase = true;
        StartCoroutine(RecordPositionHistory());
    }

    private IEnumerator RecordPositionHistory()
    {
        while (chase)
        {
            positionHistory.Add((Time.time, transform.position));
            while (positionHistory.Count > 0 && Time.time - positionHistory[0].time > positionHistoryDuration)
            {
                positionHistory.RemoveAt(0);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
