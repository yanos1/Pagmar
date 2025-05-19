using System;
using System.Collections;
using Managers;
using MoreMountains.Feedbacks;
using Obstacles;
using Player;
using SpongeScene;
using UnityEngine;

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
        
        [SerializeField] private bool canRoam = true;
        [SerializeField] private PlayerManager player;

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip charge;
        [SerializeField] private AudioClip growl;

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
        private Vector2 currentDirection = Vector2.left;
        private bool hit = false;
        private bool isKnockbacked = false;
        private const float visibilityThreshold = 0.4f;
        private float visibiliyTimer = 0f;
        private bool falling = false;
        private float chargeCooldown;
        private float flipCooldownTimer = 0f;
        private const float flipCooldownDuration = 0.5f;
        private int hitCounter = 0;
        private int hitsToKill = 2;

        public override void Start()
        {
            base.Start();
            CurrentForce = 0f;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            if (player is null)
            {
                return;
            }
            
            if (flipCooldownTimer > 0f)
                flipCooldownTimer -= Time.deltaTime;
            
            if (chargeCooldown > 0) chargeCooldown -= Time.deltaTime;
            IncrementPlayerVisibleTimer();
            
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (!IsCharging && !isPreparingCharge && distanceToPlayer < detectionRange && Mathf.Abs(player.transform.position.y- transform.position.y) < 1)
            {
                FlipTowardsPlayer();
            }
            
            if (canRoam && chargeCooldown <= 0 && !IsCharging && !isPreparingCharge && Mathf.Abs(player.transform.position.y- transform.position.y) >1 && !falling)
            {
                Roam();
            }

            if (ShouldPrepareCharge(distanceToPlayer))
            {
                isPreparingCharge = true;
                currentDirection = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;
                StartCoroutine(PrepareCharge(currentDirection));
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1, groundLayer);
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

        private void Roam()
        {

            if ((HitWall() || !GroundAhead()) && flipCooldownTimer <= 0)
            {
                print("time to switch direction 97");
                flipCooldownTimer = flipCooldownDuration;
                print($"old dir = {currentDirection} new {-currentDirection}");

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
            yield return new WaitForSeconds(chargeDelay);
            print("READY TO CHARGE AGAIN -9");
            isPreparingCharge = false;
            currentDirection = dir;

            this.StopAndStartCoroutine(ref chargeCoroutine, PerformCharge(dir));
        }

        IEnumerator PerformCharge(Vector2 dir)
        {
            CurrentForce = 1;
            float timer = 0f;
            IsCharging = true;
            while (IsCharging && timer < chargeDuration && !HitWall())
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

            src.clip = charge;
            src.Play();
        }

        private void StopCharging()
        {
            print("stop charge");
            chargeCoroutine = null;
            IsCharging = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
            chargeCooldown = chargeDelay + 0.5f;
        }

        private bool HitWall()
        {
            
            Vector2 origin = (Vector2)transform.position + Vector2.up*2 + currentDirection;
            RaycastHit2D hit = Physics2D.Raycast(origin, currentDirection, wallDetectionDistance, groundLayer);

            // Draw the ray in the Scene view
            Debug.DrawRay(origin, currentDirection * wallDetectionDistance, hit.collider ? Color.red : Color.green);

            return hit.collider != null;
        }

        
        private bool GroundAhead()
        {
            Vector2 origin = (Vector2)transform.position + currentDirection;
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
            CurrentForce = 0;
            src.Stop();
            src.clip = null;

            IsCharging = false;
            isPreparingCharge = false;
            hit = false;
            falling = false;

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.bodyType = RigidbodyType2D.Kinematic;
            }

            transform.rotation = Quaternion.identity;
            rotationTimer = 0f;
            flipCooldownTimer = 0f;
            _col.enabled = true;
        }

        public void Growl()
        {
            src.clip = growl;
            src.Play();
        }

        public void SpecialNpcRam()
        {
            IsCharging = false;
            hit = true;
            src.Stop();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.freezeRotation = false;
            _rb.AddForce(Vector2.right * 1200f);
        }

        public override void OnRam(float againstForce)
        {
            StopCharging();
            src.Stop();
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Enemy rammed with force {fromForce}");

            if (++hitCounter == hitsToKill)
            {
                gameObject.SetActive(false);
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
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.07f, () => _col.enabled = true));
            }
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb?.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}