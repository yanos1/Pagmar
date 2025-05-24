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
        agent.updateRotation = false; // Disable 3D Y-axis rotation
        agent.updateUpAxis = false;   // Enable 2D plane usage
        CurrentForce = 5;
        startingPosition = transform.position;
    }

    void Update()
    {
        if (!chase) return;

        // Move towards player
        agent.SetDestination(player.gameObject.transform.position);

        // Check proximity to resolve ram
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = new Vector2(player.gameObject.transform.position.x, player.gameObject.transform.position.y);
        float distance = Vector2.Distance(enemyPos, playerPos);

        if (distance <= contactRadius)
        {
            // RammerManager.Instance.ResolveRam(player, this);   this is done from the player now
        }

        // Rotate towards movement direction
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



    public override void OnRam(float againstForce)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public override void OnRammed(float fromForce) { }

    public override void ApplyKnockback(Vector2 direction, float force) { }

    public void ResetToInitialState()
    {
        if(!chase) return;
        agent.Warp(resetPosition);
        agent.isStopped = false;
    }

    public void StartChase()
    {
        chase = true;
    }
}
