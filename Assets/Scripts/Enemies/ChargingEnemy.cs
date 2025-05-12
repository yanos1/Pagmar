using System.Collections;
using Managers;
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
       [SerializeField] private PlayerManager player;

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip charge;
        [SerializeField] private AudioClip growl;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 1f;
        [SerializeField] private float wallDetectionDistance = 0.5f;

        private bool isCharging = false;
        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D _rb;
        private Vector2 currentDirection;
        private bool hit = false;

        public override void Start()
        {
            base.Start();
            CurrentForce = 0f;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (player is null)
            {
                return;
            }
            
            if (!isCharging)
            {
                FlipTowardsPlayer();
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            print($"distance to player is {distanceToPlayer} 9-");
            if (ShouldPrepareCharge(distanceToPlayer))
            {
                print("should prepare charge 9-");
                currentDirection = transform.position.x > player.transform.position.x ? Vector2.left : Vector2.right;
                StartCoroutine(PrepareCharge(currentDirection));
            }
        }

        private bool ShouldPrepareCharge(float distanceToPlayer)
        {
            return distanceToPlayer > minDistanceActivation &&
                   distanceToPlayer < detectionRange &&
                   !isCharging &&
                   !isPreparingCharge &&
                   player.transform.position.y < transform.position.y + 0.5f &&
                   !hit;
        }

        private void FlipTowardsPlayer()
        {
            float diffX = player.transform.position.x - transform.position.x;
            Vector2 dir = diffX > 0 ? Vector2.right : Vector2.left;
            FlipSprite(dir);
        }

        private void FlipSprite(Vector2 direction)
        {
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x > 0;
        }

        IEnumerator PrepareCharge(Vector2 dir)
        {
            if (isPreparingCharge) yield break;
            print("prepare charge 9-");
            isPreparingCharge = true;
            yield return new WaitForSeconds(chargeDelay);
            isPreparingCharge = false;
            StartCoroutine(PerformCharge(dir));
        }

        IEnumerator PerformCharge(Vector2 dir)
        {
            StartCharging();
            float timer = 0f;

            while (isCharging && timer < chargeDuration && !HitWall())
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
            print("exited while loop ");
            print($"timer : {timer} charge time {chargeDuration}");
            StopCharging();
        }

        private void StartCharging()
        {
            print("start charge 9-");

            src.clip = charge;
            src.Play();
            isCharging = true;
            CurrentForce = 1;
        }

        private void StopCharging()
        {
            print("stop charge");
            isCharging = false;
            CurrentForce = 0;
            transform.rotation = Quaternion.identity;
        }

        private bool HitWall()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.up  + currentDirection * 0.5f;
            RaycastHit2D hit = Physics2D.Raycast(origin, currentDirection, wallDetectionDistance, groundLayer);
            return hit.collider != null;
        }

        private void MoveAndRotate(Vector2 dir)
        {
            print("move!");
            transform.position += (Vector3)dir * (chargeSpeed * Time.fixedDeltaTime);
            RotateEnemy();
        }

        void FixedUpdate()
        {
            if (player != null && player.IsDead) StopAllCoroutines();
            if (!isCharging) CheckForGround();
        }

        private void RotateEnemy()
        {
            rotationTimer += Time.fixedDeltaTime * rotationSpeed;
            float rotationZ = Mathf.Sin(rotationTimer) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        private void CheckForGround()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
            if (!hitInfo.collider)
            {
                _rb.bodyType = RigidbodyType2D.Dynamic;
                StopCharging();
                isPreparingCharge = false;
                print("stopped because of no ground");
            }
            else if (_rb.bodyType != RigidbodyType2D.Kinematic)
            {
                _rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();

            src.Stop();
            src.clip = null;

            isCharging = false;
            isPreparingCharge = false;
            hit = false;

            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
                _rb.angularVelocity = 0f;
                _rb.bodyType = RigidbodyType2D.Kinematic;
            }

            transform.rotation = Quaternion.identity;
            rotationTimer = 0f;
        }

        public void Growl()
        {
            src.clip = growl;
            src.Play();
        }

        public void SpecialNpcRam()
        {
            isCharging = false;
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

            StopCharging();
            isPreparingCharge = false;
            StopAllCoroutines();

            var col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            StartCoroutine(UtilityFunctions.FadeImage(spriteRenderer, 1, 0.5f, 0.5f, () =>
            {
                StartCoroutine(UtilityFunctions.FadeImage(spriteRenderer, 0.5f, 1f, 0.5f, () =>
                {
                    if (col != null) col.enabled = true;
                }));
            }));
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            print($"add force to enemy {force } dir: {direction.normalized}");
            _rb?.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}
