using System.Collections;
using Atmosphere.TileExplostion;
using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Obstacles;
using Player;
using ScripableObjects;
using SpongeScene;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Enemies
{
    public class ChargingEnemy : Enemy
    {
        public float detectionRange;
        public float chargeSpeed;
        public float chargeDelay; // the time from when the player seens the player till a charge is discharged
        public float chargeCooldown; // time until a new charge can be done
        public float chargeDuration;
        public float minDistanceActivation;


        [SerializeField] private bool sleepAtStart;
        [SerializeField] private bool canRoam = true;
        private bool isRoaming = true;
        private bool isSleeping;
        [SerializeField] private PlayerManager player;

        [SerializeField] private GameObject sleepingImage;

        [Header("Ground Detection")] [SerializeField]
        private LayerMask groundLayer;

        [Header("Roaming Settings")] [SerializeField]
        private float groundCheckDistance = 1f;

        [SerializeField] private float wallDetectionDistance = 1f;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private EnemySpineControl spineControl;
        [SerializeField] private ChargingEnemySounds sounds;
        [SerializeField] private DoSpineFlash doSpineFlash;


        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
        private Rigidbody2D _rb;
        private Collider2D _col;
        private Coroutine flipCoroutine;
        private Coroutine chargeCoroutine;
        private Coroutine walkSoundRoutine;
        private int currentColIndex = 0;
        [SerializeField] private Vector2 currentDirection;
        private bool hit = false;
        private bool isKnockbacked = false;
        [SerializeField] private  float visibilityThreshold = 0.2f;
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
        private EventInstance sleepInstance;
        private float hitWallCd = 1f;
        private float currentHitWallCD = 0;


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
            {
                if (sleepingImage is not null)  // is actually sleeping in game and not jsut dormant.
                {
                    print("sleep animation active");
                    sleepInstance = CoreManager.Instance.AudioManager.CreateEventInstance(sounds.sleepSound);
                    sleepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

                    spineControl.PlayAnimation("sleeping",loop:true);

                    sleepInstance.start();

                }
                isSleeping = true;
            }
          

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
            if (Input.GetKeyDown(KeyCode.V))
            {
                doSpineFlash.OnRammedFeedback();
            }

            if (flipCooldownTimer > 0f)
                flipCooldownTimer -= Time.deltaTime;

            if (isPreparingCharge)
            {
                accumulatedChargePrepareTime += Time.deltaTime;
            }

            if (currentHitWallCD > 0)
            {
                currentHitWallCD -= Time.deltaTime;
            }

            if (currentChargeCooldown > 0) currentChargeCooldown -= Time.deltaTime;
            IncrementPlayerVisibleTimer();

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (!isDead && !isSleeping && !isRoaming && !IsCharging && !isPreparingCharge && distanceToPlayer < detectionRange &&
                IsPlayerVisibleLongEnough())
            {
                FlipTowardsPlayer();
            }

            if (Time.time - lastChargeTime > 1 && canRoam && !IsCharging && !isPreparingCharge && !falling && !isDead)
            {
                // print($"canRoam {canRoam} and chargeCD {chargeCooldown} and is charging {IsCharging} and is preparing chage {isPreparingCharge}");
                if (!isRoaming)
                {
                    StartPlayingWalkSound();
                }
                Roam();
            }
            else
            {
                StopPlayingWalkSound();
                isRoaming = false;
            }


            if (ShouldPrepareCharge(distanceToPlayer))
            {
                isPreparingCharge = true;
                currentDirection = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;
                StartCoroutine(PrepareCharge(currentDirection));
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.8f, groundLayer);
            Debug.DrawRay(transform.position, Vector2.up * 2.8f, hit.collider ? Color.red : Color.green);

            if (!isDead && !isRoaming && !IsCharging && !isPreparingCharge && !falling && !player.IsDead && !isKnockbacked)
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
                // print($"explosion at {transform.position}");
            }
        }

        private void OnCollisionStay2D(Collision2D col)
        {
            if (sleepAtStart && col.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                if (isSleeping)
                {
                    CoreManager.Instance.AudioManager.PlayOneShot(sounds.wakeUpSound, transform.position);

                }
                isSleeping = false;
                if (sleepingImage)
                {
                    sleepingImage.SetActive(false);
                   
                }

                if (sleepInstance.isValid())
                {
                    sleepInstance.stop(STOP_MODE.IMMEDIATE);
                    sleepInstance.release();
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
            ApplyKnockback(-currentDirection, 20);
            // _rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void WakeUp()
        {
            isSleeping = false;
            sleepInstance.stop(STOP_MODE.IMMEDIATE);
            sleepInstance.release();
        }

        private void Roam()
        {
            isRoaming = true;
            if (!spineControl.IsAnyNonLoopingAnimationPlaying())
            {
                spineControl?.PlayAnimation("walk", true);
            }

            if ((HitWall() || !GroundAhead()) && flipCooldownTimer <= 0)
            {
                flipCooldownTimer = flipCooldownDuration;

                currentDirection = -currentDirection;
                FlipSprite(currentDirection.x > 0);
            }

            transform.position += (Vector3)currentDirection * (chargeSpeed / 3 * Time.deltaTime);
        }

        private bool ShouldPrepareCharge(float distanceToPlayer)
        {
            // Debug.Log(
            //     $"[ShouldPrepareCharge Check]\n" +
            //     $"chargeCoroutine == null: {chargeCoroutine is null}\n" +
            //     $"currentChargeCooldown <= 0: {currentChargeCooldown} <= 0 → {currentChargeCooldown <= 0}\n" +
            //     $"!isDead: {!isDead}\n" +
            //     $"distanceToPlayer < detectionRange: {distanceToPlayer} < {detectionRange} → {distanceToPlayer < detectionRange}\n" +
            //     $"!IsCharging: {!IsCharging}\n" +
            //     $"!isPreparingCharge: {!isPreparingCharge}\n" +
            //     $"IsPlayerVisibleLongEnough(): {IsPlayerVisibleLongEnough()}\n" +
            //     $"!hit: {!hit}\n" +
            //     $"!player.IsDead: {!player.IsDead}\n" +
            //     $"!isKnockbacked: {!isKnockbacked}"
            // );

            return chargeCoroutine is null &&
                   currentChargeCooldown <= 0 &&
                   !isDead &&
                   distanceToPlayer < detectionRange &&
                   !IsCharging &&
                   !isPreparingCharge &&
                   IsPlayerVisibleLongEnough() &&
                   !hit &&
                   !player.IsDead &&
                   !isKnockbacked;
        }


        private bool IsPlayerVisibleLongEnough()
        {
            return visibiliyTimer >= visibilityThreshold;
        }

        private bool CheckPlayerInRoamDirection()
        {
            if (CoreManager.Instance.Player is null) return false;
            var distanceToPlayer = Mathf.Abs(CoreManager.Instance.Player.transform.position.x - transform.position.x);
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
            if(isDead) yield break;
            
            spineControl?.PlayAnimation("charge-attack", true);
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.loadCharge, transform.position);
            FlipSprite(dir.x > 0);
            print($"current charge delay {currentChargeDelay}");
            yield return new WaitForSeconds(currentChargeDelay);
            print("READY TO CHARGE AGAIN -9");
            isPreparingCharge = false;
            if(isDead) yield break;

            this.StopAndStartCoroutine(ref chargeCoroutine, PerformCharge(dir));
        }

        IEnumerator PerformCharge(Vector2 dir)
        {
            CurrentForce = 1;
            float timer = 0f;
            IsCharging = true;
            lastChargeTime = Time.time;
            spineControl?.PlayAnimation("run", true, "Idle", true);
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.chargeSound, transform.position);
            StartCoroutine(PlayRunSound());
            while (!HitWall() && IsCharging && timer < chargeDuration)
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
            if (Mathf.Abs(player.transform.position.y - transform.position.y) < 2f)
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
            detectionRange = 60f; // once charged, knows where to fijnd plyer from a distance
            chargeCoroutine = null;
            isPreparingCharge = false;
            IsCharging = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
            currentChargeCooldown = chargeCooldown;
            accumulatedChargePrepareTime = 0;
            currentChargeDelay = chargeDelay;
            currentDirection *= -1;
            FlipSprite(currentDirection.x > 0);
            print("stop charge");
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
            currentChargeDelay = chargeDelay - accumulatedChargePrepareTime;
            accumulatedChargePrepareTime = 0;
            print($"NEW CHARGE DELAY: {currentChargeDelay}");
        }

        private bool HitWall()
        {
            var hitSomething = false;
            Vector2 origin = (Vector2)transform.position + Vector2.up + currentDirection;
            RaycastHit2D raycast = Physics2D.Raycast(origin, currentDirection, wallDetectionDistance, groundLayer);

            Debug.DrawRay(origin, currentDirection * wallDetectionDistance, raycast.collider ? Color.red : Color.green);
            if (raycast.collider)
            {
                if (currentHitWallCD <= 0)
                {
                    CoreManager.Instance.AudioManager.PlayOneShot(sounds.ramWall, transform.position);
                    CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.EnemyHitWallParticles).Play(raycast.point);
                }
                hitSomething = true;
                currentHitWallCD = hitWallCd;
            }

            if (raycast.collider is not null && raycast.collider.gameObject.GetComponent<IBreakable>() is { } breakable && IsCharging)
            {
                breakable.OnBreak();
                StopCharging();
            }

            return hitSomething;
        }

        private bool GroundAhead()
        {
            Vector2 origin = (Vector2)transform.position + currentDirection * 0.2f;
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
            if(!isDead)ResetIfKnockbackOver();
        }

        private void ResetIfKnockbackOver()
        {
            if (isKnockbacked && _rb.linearVelocity.magnitude < 1 && GroundAhead())
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0;
                _rb.bodyType = RigidbodyType2D.Kinematic;
                isKnockbacked = false;
                // if (isDead)
                // {
                //     gameObject.SetActive(false);
                // }
            }
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

            if (ground && _rb.bodyType != RigidbodyType2D.Kinematic)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
                falling = false;
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            doSpineFlash.RestoreOriginalColors();
            gameObject.SetActive(true);
            StopCharging();
            StopPlayingWalkSound();
            spineControl.UnlockAnimation();
            spineControl?.PlayAnimation("Idle", true);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            currentHitWallCD = 0;

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
                if (sleepingImage is not null)  // is actually sleeping in game and not jsut dormant.
                {
                    print("sleep animation active");
                    sleepInstance = CoreManager.Instance.AudioManager.CreateEventInstance(sounds.sleepSound);
                    sleepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

                    spineControl.PlayAnimation("sleeping",loop:true);

                    sleepInstance.start();

                }
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
            spineControl?.PlayAnimation("headbutt", false, "Idle", true);
            StopCharging();
        }

        public override void OnRammed(float fromForce, Vector3 collisionPoint)
        {
            doSpineFlash.OnRammedFeedback();
            var clashPart = CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.PlayerHitEnemyParticles);
            if(CoreManager.Instance.Player.transform.position.x < transform.position.x)
            {            
                clashPart.Play(collisionPoint+Vector3.right* 1.5f);
            }
            else
            {
                clashPart.Play(collisionPoint+Vector3.left* 1.5f);
            }
       
            Debug.Log($"Enemy rammed with force {fromForce}");
            print($"hits left {hitsToKill - hitCounter + 1}");

            if (Time.time - lastRammedTime > rammedCd && ++hitCounter == hitsToKill)
            {
                lastRammedTime = Time.time;
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.damagedSound, transform.position);
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.hitSound, transform.position);
                Die();
                
                return;
            }

            hitFeedbacks?.PlayFeedbacks();
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.damagedSound, transform.position);
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.hitSound, transform.position);

            isPreparingCharge = false;
            StopAllCoroutines();
        }

        public void Die()
        {
            hitFeedbacks?.StopFeedbacks();

            spineControl.PlayAnimation("die", loop: false, onComplete: () =>
            {
                // Freeze on last frame
                spineControl.LockFinalAnimationFrame();
            });

            isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Debree");
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.deathSound, transform.position);
        }


        public override void OnTie(float fromForce, Vector3 collisionPoint)
        {
            doSpineFlash.OnRammedFeedback();
            Debug.Log($"Enemy rammed with force {fromForce}");
            print($"hits left {hitsToKill - hitCounter + 1}");

            if (Time.time - lastRammedTime > rammedCd && ++hitCounter == hitsToKill)
            {
                lastRammedTime = Time.time;
                Die();
                
                return;
            }
            isPreparingCharge = false;
            StopAllCoroutines();

            hitFeedbacks?.PlayFeedbacks();
        }
        

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            print($"add force to enemy {force} dir: {direction.normalized}");
            AbortCharge();
            isKnockbacked = true;
            // _col.enabled = false;
            // if (gameObject.activeInHierarchy)
            // {
            //     StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.02f, () => _col.enabled = true));
            // }

            _rb.bodyType = RigidbodyType2D.Dynamic;
            
            _rb?.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }

        public void StartPlayingWalkSound()
        {
            if (walkSoundRoutine == null)
            {
                walkSoundRoutine = StartCoroutine(PlayWalkSoundLoop());
            }
        }

        public void StopPlayingWalkSound()
        {
            if (walkSoundRoutine != null)
            {
                StopCoroutine(walkSoundRoutine);
                walkSoundRoutine = null;
            }
        }

        private IEnumerator PlayWalkSoundLoop()
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, player.transform.position) > 20)
                {
                    StopPlayingWalkSound();
                }
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.walkSound, transform.position);
                yield return new WaitForSeconds(1f);
            }
        }
        
        private IEnumerator PlayRunSound()
        {
            while (IsCharging)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.walkSound, transform.position);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}