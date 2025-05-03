using Managers;
using UnityEngine;

namespace Enemies
{
    public class FlyingEnemy : Enemy
    {
        public enum State { Idle, Roaming, Chasing, Attacking }
        public State currentState = State.Idle;

        public float detectionRange = 20f;
        public float chaseRange = 8f;
        public float roamSpeed = 1.5f;
        public float chaseSpeed = 2.5f;
        public float attackSpeed = 10f;
        public float attackCooldown = 5f;
        public float floatMagnitude = 1f;
        public float attackDuration = 0.5f;
        private Vector3 chaseDirection;
        private float chaseDirectionTimer;
        private float nextDirectionUpdateTime;

        private Transform player;
        private float attackTimer = 0f;
        private float attackTimeElapsed = 0f;
        private Vector3 attackDirection;
        private Vector3 previousToPlayerDir;
        private float verticalVelocity = 1.3f;


        // New variables
        private Vector3 startPosition;
        private float roamHeightLimit = 1.2f; // Limit to how much the enemy can float vertically

        public override void Start()
        {
            base.Start();
            player = CoreManager.Instance.Player.transform;
            startPosition = transform.position; // Initialize start position
        }

        public override void OnRam()
        {
            gameObject.SetActive(false);
        }

        public override bool IsDeadly()
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
                    {
                        currentState = State.Roaming;
                    }
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
                    {
                        currentState = State.Idle;
                    }
                    break;

                case State.Chasing:
                    Chase();
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= attackCooldown)
                    {
                        currentState = State.Attacking;
                        attackTimeElapsed = 0f;
                        attackDirection = (player.position - transform.position).normalized;
                    }
                    if (distance >= detectionRange)
                    {
                        currentState = State.Idle;
                    }
                    break;

                case State.Attacking:
                    Attack();
                    break;
            }
            print(currentState);
        }

        void Roam()
        {
            // Perlin noise for smooth random movement
            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(Time.time, 0) - 0.5f,
                Mathf.PerlinNoise(0, Time.time) - 0.5f,
                0
            );
            transform.position += noise * (floatMagnitude * roamSpeed * Time.deltaTime);

            // Ensure vertical movement doesn't exceed the limit
            ClampVerticalPosition();
        }

        void Chase()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;

            // Check for sudden player position change
            if ((toPlayer.x < 0 && chaseDirection.x > 0 || toPlayer.x > 0 && chaseDirection.x < 0)  && nextDirectionUpdateTime > 0.2f)
            {
                print("332change dir");
                nextDirectionUpdateTime = 0.2f;
            }


            // Update direction
            chaseDirectionTimer += Time.deltaTime;
            if (chaseDirectionTimer >= nextDirectionUpdateTime)
            {
                SetNewChaseDirection();
            }

            // Add small Perlin noise
            Vector3 noise = new Vector3(
                Mathf.PerlinNoise(Time.time * 0.8f, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, Time.time * 0.8f) - 0.5f,
                0f
            );

            Vector3 movement = (chaseDirection + noise * 0.4f).normalized * chaseSpeed * Time.deltaTime;
            transform.position += movement;

            ClampVerticalPosition();
        }

        void Attack()
        {
            attackTimeElapsed += Time.deltaTime;
            transform.position += attackDirection * (attackSpeed * Time.deltaTime);

            // After attack duration, switch back to chasing
            if (attackTimeElapsed >= attackDuration)
            {
                currentState = State.Chasing;
                attackTimer = 0f;
            }
        }

        void SetNewChaseDirection()
        {
            Vector3 toPlayer = (player.position - transform.position).normalized;

            // Try until we get a direction with dot product between 0.9–1.0
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

            // Schedule next update
            chaseDirectionTimer = 0f;
            nextDirectionUpdateTime = Random.Range(3f, 5f);
        }


        void ClampVerticalPosition()
        {
            float lowerBound = startPosition.y - roamHeightLimit;
            float upperBound = startPosition.y + roamHeightLimit;
            float targetY = Mathf.Clamp(transform.position.y, lowerBound, upperBound);

            float smoothedY = Mathf.SmoothDamp(transform.position.y, targetY, ref verticalVelocity, 0.2f); // 0.2f is smooth time
            transform.position = new Vector3(transform.position.x, smoothedY, transform.position.z);
        }

    }
}
