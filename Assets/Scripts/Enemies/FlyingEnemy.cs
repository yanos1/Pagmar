using System;
using System.Collections;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class FlyingEnemy : Enemy
    {
        public enum State { Idle, Roaming, Chasing, PrepareAttack, Attacking }
        public State currentState = State.Idle;

        public float detectionRange = 20f;
        public float chaseRange = 8f;
        public float roamSpeed = 1.5f;
        public float chaseSpeed = 2.5f;
        public float attackSpeed = 10f;
        public float attackCooldown = 5f;
        public float floatMagnitude = 1f;
        public float attackDuration = 0.5f;
        public float prepareAttackDelay = 0.3f;

        [SerializeField] private Transform player;
        [SerializeField] private MMF_Player prepareAttackFeedbacks;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private GameObject healingRune;
        [SerializeField] private bool healer = false;
        

        private float roamHeightLimit = 1.2f;
        private float verticalVelocity = 1.3f;
        private float attackTimer = 0f;
        private float attackTimeElapsed = 0f;
        private float prepareAttackTimer = 0f;

        private Vector3 chaseDirection;
        private float chaseDirectionTimer;
        private float nextDirectionUpdateTime;
        private Vector3 attackDirection;

        private Rigidbody2D rb;
        private Coroutine knockbackCoroutine;

        public override void Awake()
        {
            base.Awake();
            CurrentForce = 0;
            MaxForce = 1;
            rb = GetComponent<Rigidbody2D>();
        }

     
        public bool IsDeadly()
        {
            return currentState == State.Attacking;
        }

        void Update()
        {
            float distance = Vector3.Distance(transform.position, player.position);

            switch (currentState)
            {
                case State.Idle:
                    if (distance < detectionRange && distance >= chaseRange)
                        currentState = State.Roaming;
                    else if (distance < chaseRange)
                    {
                        currentState = State.Chasing;
                        attackTimer = 0f;
                    }
                    break;

                case State.Roaming:
                    Roam();
                    if (distance < chaseRange)
                    {
                        currentState = State.Chasing;
                        attackTimer = 0f;
                    }
                    else if (distance >= detectionRange)
                        currentState = State.Idle;
                    break;

                case State.Chasing:
                    Chase();
                    attackTimer += Time.deltaTime;

                    if (attackTimer >= attackCooldown)
                    {
                        currentState = State.PrepareAttack;
                        prepareAttackTimer = 0f;
                        attackDirection = (player.position - transform.position).normalized;
                        trail.enabled = true;
                        prepareAttackFeedbacks?.PlayFeedbacks();
                    }
                    if (distance >= detectionRange)
                        currentState = State.Idle;
                    break;

                case State.PrepareAttack:
                    prepareAttackTimer += Time.deltaTime;
                    if (prepareAttackTimer >= prepareAttackDelay)
                    {
                        currentState = State.Attacking;
                        attackTimeElapsed = 0f;
                    }
                    break;

                case State.Attacking:
                    Attack();
                    break;
            }
        }

        void Roam()
        {
            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(Time.time, 0) - 0.5f,
                Mathf.PerlinNoise(0, Time.time) - 0.5f,
                0
            );

            Vector3 nextPosition = transform.position + noise * (floatMagnitude * roamSpeed * Time.deltaTime);
            nextPosition = ClampVerticalPosition(nextPosition);
            rb.MovePosition(nextPosition);
        }

        void Chase()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;

            if ((toPlayer.x < 0 && chaseDirection.x > 0 || toPlayer.x > 0 && chaseDirection.x < 0) && nextDirectionUpdateTime > 0.2f)
                nextDirectionUpdateTime = 0.2f;

            chaseDirectionTimer += Time.deltaTime;
            if (chaseDirectionTimer >= nextDirectionUpdateTime)
                SetNewChaseDirection();

            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(Time.time * 0.8f, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * 0.8f) - 0.5f,
                0f
            );

            Vector3 movement = (chaseDirection + noise * 0.4f).normalized * (chaseSpeed * Time.deltaTime);
            Vector3 nextPosition = transform.position + movement;
            nextPosition = ClampVerticalPosition(nextPosition);
            rb.MovePosition(nextPosition);
        }

        void Attack()
        {
            attackTimeElapsed += Time.deltaTime;
            CurrentForce = 0.6f;
            Vector3 nextPosition = transform.position + attackDirection * (attackSpeed * Time.deltaTime);
            rb.MovePosition(nextPosition);

            if (attackTimeElapsed >= attackDuration)
            {
                CurrentForce = 0;
                trail.enabled = false;
                currentState = State.Chasing;
                attackTimer = 0f;
            }
        }

        void SetNewChaseDirection()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;

            for (int i = 0; i < 10; i++)
            {
                Vector2 randomOffset = Random.insideUnitCircle.normalized * 0.5f;
                Vector3 candidate = (toPlayer + new Vector3(randomOffset.x, randomOffset.y, 0f)).normalized;

                if (Vector3.Dot(toPlayer, candidate) >= 0.9f)
                {
                    chaseDirection = candidate;
                    break;
                }
            }

            chaseDirectionTimer = 0f;
            nextDirectionUpdateTime = Random.Range(3f, 5f);
        }

        Vector3 ClampVerticalPosition(Vector3 position)
        {
            float lowerBound = startingPos.y - roamHeightLimit;
            float upperBound = startingPos.y + roamHeightLimit;
            float clampedY = Mathf.Clamp(position.y, lowerBound, upperBound);
            float smoothedY = Mathf.SmoothDamp(transform.position.y, clampedY, ref verticalVelocity, 0.2f);
            return new Vector3(position.x, smoothedY, position.z);
        }

        public override void OnRam(Vector2 ramDirNegative, float againstForce)
        {
            hitFeedbacks?.PlayFeedbacks();
        }

        public override void OnRammed(float fromForce)
        {
            if (healer)
            {
                Instantiate(healingRune, transform.position,Quaternion.identity);
            }
            gameObject.SetActive(false);
        }

        public override void OnTie(float fromForce)
        {
            return;
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            if (knockbackCoroutine != null)
                StopCoroutine(knockbackCoroutine);

            knockbackCoroutine = StartCoroutine(KnockbackRoutine(direction, force));
        }

        private IEnumerator KnockbackRoutine(Vector2 direction, float force)
        {
            currentState = State.Idle;
            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = 1f - (elapsed / duration);
                rb.linearVelocity = direction * force * t;
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            knockbackCoroutine = null;
        }
    }
    
}
