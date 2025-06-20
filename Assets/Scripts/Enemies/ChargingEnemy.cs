using System;
using System.Collections;
using Interfaces;
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
        public float chargeDelay;  // the time from when the player seens the player till a charge is discharged
        public float chargeCooldown; // time until a new charge can be done
        public float chargeDuration;
        public float rotationAmount = 10f;
        public float rotationSpeed = 5f;
        public float minDistanceActivation = 3f;

        [SerializeField] private bool sleepAtStart;
        [SerializeField] private bool canRoam = true;
        private bool isRoaming = true;
        private bool isSleeping;
        [SerializeField] private PlayerManager player;

        [SerializeField] private GameObject sleepingImage;

        [Header("Ground Detection")] [SerializeField]
        private LayerMask groundLayer;

        [Header("Roaming Settings")]
        
        [SerializeField] private float groundCheckDistance = 1f;
        [SerializeField] private float wallDetectionDistance = 1f;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private EnemySpineControl spineControl;


        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
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
        private float lastChargeTime = 0f;
        private float flipCooldownTimer = 0f;
        private const float flipCooldownDuration = 0.5f;
        private int hitCounter = 0;
        [SerializeField] private int hitsToKill = 2;
        private float startDetectionRange;
        private Vector2 baseDir;
        private bool initialized = false;
        private float rammedCd = 0.5f;
        private float lastRammedTime;
        private bool isDead = false;
        private float accumulatedChargePrepareTime = 0;
        private float currentChargeCooldown;
        private float currentChargeDelay;


        public bool IsDead => isDead;
        // private void OnEnable()
        // {
        //     if (!initialized)
        //     {
        //         Start();
        //         initialized = true;
        //     } 
        // }

        public void Awake()
        {
            base.Awake();
            CurrentForce = 0f;
            startDetectionRange = detectionRange;
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<BoxCollider2D>();

            if (sleepAtStart)
                isSleeping = true;

            baseDir = currentDirection;

            // Flip scale based on initial direction
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (currentDirection == Vector2.right ? 1 : -1);
            transform.localScale = scale;
            currentChargeDelay = chargeDelay;
            
        }


        void Update()
        {
            if (player is null || isSleeping)
            {
                return;
            }

            if (flipCooldownTimer > 0f)
                flipCooldownTimer -= Time.deltaTime;

            if (isPreparingCharge)
            {
                accumulatedChargePrepareTime += Time.deltaTime;
            }

            if (currentChargeCooldown > 0) currentChargeCooldown -= Time.deltaTime;
            IncrementPlayerVisibleTimer();

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (!isSleeping && !isRoaming && !IsCharging && !isPreparingCharge && distanceToPlayer < detectionRange && IsPlayerVisibleLongEnough())
            {
                FlipTowardsPlayer();
            }
           
            if ( Time.time - lastChargeTime > 1 && canRoam && !IsCharging && !isPreparingCharge && !falling)
            {
                // print($"canRoam {canRoam} and chargeCD {chargeCooldown} and is charging {IsCharging} and is preparing chage {isPreparingCharge}");
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
            if (!isRoaming && !IsCharging && !isPreparingCharge && !falling && !player.IsDead && !isKnockbacked)
            {
                if (!spineControl.IsAnyNonLoopingAnimationPlaying())
                {
                    spineControl?.PlayAnimation("Idle", true);
                }
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
        private void OnCollisionStay2D(Collision2D col)
        {
            if (sleepAtStart  && col.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                isSleeping = false;
                if (sleepingImage)
                { 
                    sleepingImage.SetActive(false);
                }
            }

            // if (col.gameObject.GetComponent<PlayerMovement>() is {} player)
            // {
            //     // Attach player to platform
            //     if (player.GroundCheckPos.y - 0.2f > transform.position.y)
            //     {
            //         col.transform.SetParent(transform);
            //     }
            // }



        }

        public void AffectedByExternalKnockback()
        {
            print("cancel charge 77");
            ApplyKnockback(-currentDirection, 25);
            // _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void WakeUp()
        {
            isSleeping = false;
        }
        private void Roam()
        {
            isRoaming = true;
            if (!spineControl.IsAnyNonLoopingAnimationPlaying())
            {
                spineControl?.PlayAnimation("walk", true);
            }

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
            return chargeCoroutine is null && currentChargeCooldown <= 0 &&
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
            var distanceToPlayer = Mathf.Abs(CoreManager.Instance.Player.transform.position.x- transform.position.x);
            if (distanceToPlayer > minDistanceActivation) return false;
            Vector2 toPlayer = CoreManager.Instance.Player.transform.position - transform.position;
            
            float dot = Vector2.Dot(toPlayer.normalized, currentDirection.normalized);

            // dot > 0 means player is in the current direction (in front)
            if (dot > 0.5f && distanceToPlayer < 2) // adjust tolerance as needed
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
            bool newFacingRight = direction.x > 0;
            bool currentlyFacingRight = transform.localScale.x > 0;

            if (currentlyFacingRight != newFacingRight && flipCoroutine == null)
            {
                flipCoroutine = StartCoroutine(FlipSpriteAfterDelay(newFacingRight));
            }
        }

        private IEnumerator FlipSpriteAfterDelay(bool faceRight)
        {
            yield return new WaitForSeconds(currentChargeDelay);
            FlipSprite(faceRight);
        }

        private void FlipSprite(bool faceRight)
        {
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (faceRight ? 1 : -1);
            transform.localScale = localScale;
            flipCoroutine = null;
        }

        IEnumerator PrepareCharge(Vector2 dir)
        {
            StartCharging();
            
            FlipSprite(dir.x >0);
            print($"current charge delay {currentChargeDelay}");
            yield return new WaitForSeconds(currentChargeDelay);
            print("READY TO CHARGE AGAIN -9");
            isPreparingCharge = false;

            this.StopAndStartCoroutine(ref chargeCoroutine, PerformCharge(dir));
        }

        IEnumerator PerformCharge(Vector2 dir)
        {
            CurrentForce = 1;
            float timer = 0f;
            IsCharging = true;
            lastChargeTime = Time.time;
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
            if (  Mathf.Abs(player.transform.position.y - transform.position.y) < 2f)
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
            spineControl?.PlayAnimation("run", true);
        }

        private void StopCharging()
        {
            print("stop charge");
            detectionRange = 60f; // once charged, knows where to fijnd plyer from a distance
            chargeCoroutine = null;
            isPreparingCharge = false;
            IsCharging = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
            currentChargeCooldown = chargeCooldown;
            accumulatedChargePrepareTime = 0;
            currentChargeDelay = chargeDelay;
        }
        private void AbortCharge()
        {
            print("Abort charge");
            detectionRange = 60f; // once charged, knows where to fijnd plyer from a distance
            chargeCoroutine = null;
            IsCharging = false;
            isPreparingCharge = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
            currentChargeDelay = chargeDelay-accumulatedChargePrepareTime;
            print($"NEW CHARGE CD: {chargeDelay}");
        }

        private bool HitWall()
        {
            var hitSomething = false;
            Vector2 origin = (Vector2)transform.position + Vector2.up + currentDirection;
            RaycastHit2D raycast = Physics2D.Raycast(origin, currentDirection, wallDetectionDistance, groundLayer);

            Debug.DrawRay(origin, currentDirection * wallDetectionDistance, raycast.collider ? Color.red : Color.green);
            if (raycast.collider)
            {
                hitSomething = true;
            }
            if (raycast.collider is not null && raycast.collider.gameObject.GetComponent<IBreakable>() is { } breakable)
            {
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
            _rb.MovePosition(_rb.position + dir * (chargeSpeed * Time.fixedDeltaTime));
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
                if (isDead)
                {
                    gameObject.SetActive(false);
                }
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
            StopCharging();

            // Flip using scale (Spine-compatible)
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(transform.localScale.x) * (baseDir == Vector2.right ? 1 : -1);
            transform.localScale = scale;

            currentDirection = baseDir;
            detectionRange = startDetectionRange;
            isKnockbacked = false;
            hit = false;
            hitCounter = 0;
            falling = false;
            isDead = false;

            if (sleepAtStart)
            {
                if (sleepingImage) sleepingImage.SetActive(true);
                isSleeping = true;
            }

            // hitFeedbacks?.StopFeedbacks(); // return to this!

            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;

            rotationTimer = 0f;
            flipCooldownTimer = 0f;
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
            spineControl?.PlayAnimation("headbutt", false, "Idle",true);
            StopCharging();
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Enemy rammed with force {fromForce}");
            print($"hits left {hitsToKill- hitCounter +1}");

            if (Time.time - lastRammedTime > rammedCd && ++hitCounter == hitsToKill)
            {
                lastRammedTime = Time.time;
                hitFeedbacks?.StopFeedbacks();
                isDead = true;
                return;
            }

            hitFeedbacks?.PlayFeedbacks();

            isPreparingCharge = false;
            StopAllCoroutines();
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            print($"add force to enemy {force} dir: {direction.normalized}");
            AbortCharge();
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