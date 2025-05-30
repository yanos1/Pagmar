using System;
using System.Collections;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Obstacles;
using Player;
using SpongeScene;
using Terrain.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class ChargingEnemy : Enemy
    {
        public float detectionRange;
        public float chargeSpeed;
        public float chargeDelay;
        public float chargeDuration;
        public float rotationAmount = 10f;
        public float rotationSpeed = 5f;
        public float minDistanceActivation = 3f;

        [SerializeField] private bool sleepAtStart;
        [SerializeField] private bool canRoam = true;
        private bool isRoaming = true;
        private bool isSleeping;
        [SerializeField] private PlayerManager player;

        [FormerlySerializedAs("sleeping")] [SerializeField] private GameObject sleepingImage;

        [Header("Ground Detection")] [SerializeField]
        private LayerMask groundLayer;

        [Header("Roaming Settings")]
        
        [SerializeField] private float groundCheckDistance = 1f;
        [SerializeField] private float wallDetectionDistance = 1f;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private MMF_Player hitFeedbacks;


        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D _rb;
        private Collider2D _col;
        private Coroutine flipCoroutine;
        private Coroutine chargeCoroutine;
        private int currentColIndex = 0;
        [SerializeField] private Vector2 currentDirection;
        private bool hit = false;
        private bool isKnockbacked = false;
        private const float visibilityThreshold = 0.6f;
        private float visibiliyTimer = 0f;
        private bool falling = false;
        private float chargeCooldown;
        private float flipCooldownTimer = 0f;
        private const float flipCooldownDuration = 0.5f;
        private int hitCounter = 0;
        private int hitsToKill = 2;
        private float startDetectionRange;
        private Vector2 baseDir;
        private bool baseFlip = false;

        private void OnEnable()
        {
            Start();
        }

        public override void Start()
        {
            base.Start();
            CurrentForce = 0f;
            startDetectionRange = detectionRange;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
            if (sleepAtStart) isSleeping = true;
            baseDir = currentDirection;
            baseFlip = spriteRenderer.flipX;
        }

        void Update()
        {
            if (player is null || isSleeping)
            {
                return;
            }

            if (flipCooldownTimer > 0f)
                flipCooldownTimer -= Time.deltaTime;

            if (chargeCooldown > 0) chargeCooldown -= Time.deltaTime;
            IncrementPlayerVisibleTimer();

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (!isSleeping && !isRoaming && !IsCharging && !isPreparingCharge && distanceToPlayer < detectionRange && IsPlayerVisibleLongEnough())
            {
                FlipTowardsPlayer();
            }
           
            if (canRoam && chargeCooldown <= 0 && !IsCharging && !isPreparingCharge && !falling)
            {
                
                Roam();
            }
            else
            {
                isRoaming = false;
            }


        if (ShouldPrepareCharge(distanceToPlayer))
            {
                isPreparingCharge = true;
                currentDirection = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;
                StartCoroutine(PrepareCharge(currentDirection));
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.8f, groundLayer);
            Debug.DrawRay(transform.position, Vector2.up* 2.8f, hit.collider ? Color.red : Color.green);

            if (hit.collider is not null && (hit.collider.gameObject.GetComponent<GuillotineTrap>() is not null))
            {
                e.explode();
                f.doExplosion(transform.position);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<AlternatingLavaBeam>() is not null)
            {
                e.explode();
                print($"explosion at {transform.position}");
                f.doExplosion(transform.position);
            }
        }
        
        private void OnCollisionEnter2D(Collision2D col)
        {
            print($"enemy collided with {col.gameObject.name} l0");
            if (sleepAtStart  && col.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                isSleeping = false;
                if (sleepingImage)
                { 
                    sleepingImage.SetActive(false);
                }
            }
            
        }

        private void Roam()
        {
            isRoaming = true;
            if ((HitWall() || !GroundAhead() || CheckPlayerInRoamDirection()) && flipCooldownTimer <= 0)
            {
                flipCooldownTimer = flipCooldownDuration;

                currentDirection = -currentDirection;
                FlipSprite(currentDirection.x > 0);
            }

            transform.position += (Vector3)currentDirection * (chargeSpeed/3 * Time.deltaTime);
        }


        private bool  ShouldPrepareCharge(float distanceToPlayer)
        {
            return chargeCoroutine is null && distanceToPlayer > minDistanceActivation &&
                   distanceToPlayer < detectionRange &&
                   !IsCharging &&
                   !isPreparingCharge &&
                   IsPlayerVisibleLongEnough() &&
                   !hit && !player.IsDead && !isKnockbacked;
        }

        private bool IsPlayerVisibleLongEnough()
        {
            return visibiliyTimer >= visibilityThreshold;
        }
        
        private bool CheckPlayerInRoamDirection()
        {
            if (CoreManager.Instance.Player is  null) return false;
            var distanceToPlayer = Vector2.Distance(CoreManager.Instance.Player.transform.position, transform.position);
            if (distanceToPlayer > minDistanceActivation) return false;
            Vector2 toPlayer = CoreManager.Instance.Player.transform.position - transform.position;
            
            float dot = Vector2.Dot(toPlayer.normalized, currentDirection.normalized);

            // dot > 0 means player is in the current direction (in front)
            if (dot > 0.5f) // adjust tolerance as needed
            {
                return true;
            }

            return false;
        }

        private void FlipTowardsPlayer()
        {
            float diffX = player.transform.position.x - transform.position.x;
            Vector2 dir = diffX > 0 ? Vector2.right : Vector2.left;
            FlipSpriteWithDelay(dir);
        }

        private void FlipSpriteWithDelay(Vector2 direction)
        {
            bool newFlipX = direction.x > 0;

            // Only apply flip logic if it actually changes
            if (spriteRenderer.flipX != newFlipX && flipCoroutine is null)
            {
                flipCoroutine = StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.5f, () =>
                {
                    FlipSprite(newFlipX);
                }));
            }
        }

        private void FlipSprite(bool newFlipX)
        {
            print($"old 87 {_col.offset}");
            
            spriteRenderer.flipX = newFlipX;
            var currentOffset = _col.offset;
            currentOffset.x *= -1;
            _col.offset = currentOffset;
            print($"new 87 {_col.offset}");
            flipCoroutine = null;
        }

        IEnumerator PrepareCharge(Vector2 dir)
        {
            StartCharging();
            
            FlipTowardsPlayer();
            yield return new WaitForSeconds(chargeDelay);
            print("READY TO CHARGE AGAIN -9");
            isPreparingCharge = false;

            this.StopAndStartCoroutine(ref chargeCoroutine, PerformCharge(dir));
        }

        IEnumerator PerformCharge(Vector2 dir)
        {
            CurrentForce = 1;
            float timer = 0f;
            IsCharging = true;
            while ( !HitWall() && IsCharging && timer < chargeDuration)
            {
                MoveAndRotate(dir);
                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
                if (Mathf.Approximately(player.transform.position.x, transform.position.x))
                {
                    print("too close 999");
                    StopAllCoroutines();
                    break;
                }
            }

            print($"exited while loop , is charging :{IsCharging}, hit wall {HitWall()}");
            print($"timer : {timer} charge time {chargeDuration}");
            StopCharging();
        }


        private void IncrementPlayerVisibleTimer()
        {
            if (  Mathf.Abs(player.transform.position.y - transform.position.y) < 0.5f)
            {
                visibiliyTimer += Time.deltaTime;
            }
            else
            {
                visibiliyTimer = 0f;
            }
        }

        private void StartCharging()
        {
            print("start charge 9-");

        }

        private void StopCharging()
        {
            print("stop charge");
            detectionRange = 60f; // once charged, knows where to fijnd plyer from a distance
            chargeCoroutine = null;
            IsCharging = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
            chargeCooldown = chargeDelay + 0.5f;
        }

        private bool HitWall()
        {
            var hitSomething = false;
            Vector2 origin = (Vector2)transform.position + Vector2.up + currentDirection;
            RaycastHit2D raycast = Physics2D.Raycast(origin, currentDirection, wallDetectionDistance, groundLayer);

            Debug.DrawRay(origin, currentDirection * wallDetectionDistance, raycast.collider ? Color.red : Color.green);
            if (raycast.collider)
            {
                print($"collider is {raycast.collider}");
                hitSomething = true;
            }
            if (raycast.collider is not null && raycast.collider.gameObject.GetComponent<IBreakable>() is { } breakable)
            {
                print("break!");
                breakable.OnBreak();
                gameObject.SetActive(false);
            }

            return hitSomething;

        }
        
        private bool GroundAhead()
        {
            Vector2 origin = (Vector2)transform.position + currentDirection* 0.2f;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance + 0.1f, groundLayer);
            return hit.collider != null;
        }

        private void MoveAndRotate(Vector2 dir)
        {
            transform.position += (Vector3)dir * (chargeSpeed * Time.fixedDeltaTime);
            RotateEnemy();
        }

        void FixedUpdate()
        {
            if (player != null && player.IsDead) StopAllCoroutines();
            if (IsCharging && !isKnockbacked) CheckForGround();
            ResetIfKnockbackOver();
        }

        private void ResetIfKnockbackOver()
        {
            if (isKnockbacked && _rb.linearVelocity.magnitude < 1 && GroundAhead())
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0;

                _rb.bodyType = RigidbodyType2D.Kinematic;
                isKnockbacked = false;
            }
        }

        private void RotateEnemy()
        {
            rotationTimer += Time.fixedDeltaTime * rotationSpeed;
            float rotationZ = Mathf.Sin(rotationTimer) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        private void CheckForGround()
        {
            var ground = GroundAhead();
            if (!ground && _rb.bodyType == RigidbodyType2D.Kinematic)
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
                StopCharging();
                isPreparingCharge = false;
                falling = true;
                print("stopped because of no ground");
            }
            
            if(ground && _rb.bodyType != RigidbodyType2D.Kinematic)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
                falling = false;
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            gameObject.SetActive(true);
            CurrentForce = 0;
            spriteRenderer.flipX = baseFlip;
            currentDirection = baseDir;
            detectionRange = startDetectionRange;
            IsCharging = false;
            isPreparingCharge = false;
            hit = false;
            hitCounter = 0;
            falling = false;
            if (sleepAtStart)
            {
                sleepingImage.SetActive(true);
                isSleeping = true;
            }
            // hitFeedbacks?.StopFeedbacks(); // return to this!

            
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            

            transform.rotation = Quaternion.identity;
            rotationTimer = 0f;
            flipCooldownTimer = 0f;
            chargeCooldown = 0f;
            _col.enabled = true;
        }

        public void Growl()
        {
        }

        public void SpecialNpcRam()
        {
            IsCharging = false;
            hit = true;
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.freezeRotation = false;
            _rb.AddForce(Vector2.right * 1200f);
        }

        public override void OnRam(Vector2 ramDiNegative, float againstForce)
        {
            StopCharging();
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Enemy rammed with force {fromForce}");

            if (++hitCounter == hitsToKill)
            {
                gameObject.SetActive(false);
                return;
            }

            hitFeedbacks?.PlayFeedbacks();

            StopCharging();
            isPreparingCharge = false;
            StopAllCoroutines();
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            print($"add force to enemy {force} dir: {direction.normalized}");
            StopCharging();
            isKnockbacked = true;
            _col.enabled = false;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.05f, () => _col.enabled = true));
            }
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb?.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}