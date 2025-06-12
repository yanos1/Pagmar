using System;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Managers;

public class ChasingEnemy : Rammer, IResettable
{
    [SerializeField] private Rammer player;
    private float contactRadius;
    private NavMeshAgent agent;
    private float rotationSpeed = 3f;
    private Collider2D col;
    private Vector3 startingPosition;
    private Vector3 resetPosition;
    private bool chase = false;

    private float positionHistoryDuration = 2.6f;
    private List<(float time, Vector3 position)> positionHistory = new List<(float, Vector3)>();

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
    }

    private void OnReachedCheckpoint(object obj)
    {
        if (obj is Vector3 pos)
        {
            resetPosition = transform.position;
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

        // Track position history for 2-second rewind
        positionHistory.Add((Time.time, transform.position));
        while (positionHistory.Count > 0 && Time.time - positionHistory[0].time > positionHistoryDuration)
        {
            positionHistory.RemoveAt(0);
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

    public override void OnRammed(float fromForce) { }

    public override void ApplyKnockback(Vector2 direction, float force) { }

    public void ResetToInitialState()
    {
        if (!chase) return;

        Vector3 targetResetPosition;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < 3f && positionHistory.Count > 0)
        {
            targetResetPosition = positionHistory[0].position;
        }
        else
        {
            targetResetPosition = resetPosition;
        }

        // Apply the warp
        agent.Warp(targetResetPosition);
        agent.isStopped = false;

        // Clear and seed position history with the new reset position
        positionHistory.Clear();
        positionHistory.Add((Time.time - positionHistoryDuration, targetResetPosition));
    }

    public void StartChase()
    {
        chase = true;
    }
}
