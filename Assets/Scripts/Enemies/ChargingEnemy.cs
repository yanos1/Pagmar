using System;
using System.Collections;
using Managers;
using NPC;
using Player;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Transform player;

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip charge;
        [SerializeField] private AudioClip growl;

        [Header("Ground Detection")] [SerializeField]
        private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 1f;

        private bool isCharging = false;
        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D _rb;
        private Vector2 currentDirection = Vector2.right;
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
            if (player == null)
            {
                Debug.LogWarning("Player reference is missing!");
                return;
            }

            if (Mathf.Abs(transform.position.x - player.position.x) > 1)
            {
                FlipSprite(currentDirection);
            }
            else if (Mathf.Abs(transform.position.x - player.position.x) < -1)
            {
                FlipSprite(currentDirection);
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > minDistanceActivation && distanceToPlayer < detectionRange && !isCharging &&
                !isPreparingCharge && player.transform.position.y < transform.position.y + 0.5f && !hit)
            {
                print($"preparing charge since distance is {distanceToPlayer} and detection range is {detectionRange}");
                StartCoroutine(PrepareCharge());
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
            currentDirection = Vector2.right;
        }

        // public override bool IsDeadly()
        // {
        //     return isCharging && player.transform.position.y - 2.2f < transform.position.y;
        // }

        IEnumerator PrepareCharge()
        {
            isPreparingCharge = true;
            yield return new WaitForSeconds(chargeDelay);
            isPreparingCharge = false;
            StartCharge();
        }

        void StartCharge()
        {
            src.clip = charge;
            src.Play();
            isCharging = true;
            CurrentForce = 1;
            StartCoroutine(ChargeCooldown());
        }

        IEnumerator ChargeCooldown()
        {
            yield return new WaitForSeconds(chargeDuration);
            isCharging = false;
            CurrentForce = 0;
        }

        void FixedUpdate()
        {
            currentDirection = (player.position - transform.position).x > 0 ? Vector2.right : Vector2.left;

            if (isCharging)
            {
                transform.position += (Vector3)currentDirection * (chargeSpeed * Time.fixedDeltaTime);
                RotateEnemy();
                
            }

            CheckForGround();
        }

        void RotateEnemy()
        {
            rotationTimer += Time.fixedDeltaTime * rotationSpeed;
            float rotationZ = Mathf.Sin(rotationTimer) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        }

        void FlipSprite(Vector2 direction)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x > 0;
            }
        }

        private void CheckForGround()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
            if (!hitInfo.collider)
            {
                if (_rb.bodyType != RigidbodyType2D.Dynamic)
                {
                    _rb.bodyType = RigidbodyType2D.Dynamic;
                    isCharging = false;
                    CurrentForce = 0;
                    isPreparingCharge = false;
                }
            }
            else
            {
                if (_rb.bodyType != RigidbodyType2D.Kinematic)
                {
                    _rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }

        public void Growl()
        {
            src.clip = growl;
            src.Play();
        }
        
        // this func should be replecad when possible. its a placeholder
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
            isCharging = false;
            src.Stop();
            CurrentForce = 0;
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Enemy rammed with force {fromForce}");
            
            isCharging = false;
            isPreparingCharge = false;
            StopAllCoroutines();
            
            // Disable collider
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Start fade-out coroutine
            StartCoroutine(UtilityFunctions.FadeImage(gameObject.GetComponent<SpriteRenderer>(),1, 0.5f, 0.5f,
                () =>
                {
                    StartCoroutine(UtilityFunctions.FadeImage(gameObject.GetComponent<SpriteRenderer>(), 0.5f, 1f, 0.5f,
                        () =>    col.enabled = true));
                }));
        }


        public override void ApplyKnockback(Vector2 direction, float force)
        {
            if (_rb != null)
            {
                _rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            }
        }
    }
}
