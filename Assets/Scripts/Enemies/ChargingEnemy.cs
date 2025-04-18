using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Enemies
{
    public class ChargingEnemy : Enemy
    {
        public float detectionRange;
        public float chargeSpeed;
        public float chargeDelay;
        public float chargeCooldown;
        public float rotationAmount = 10f;
        public float rotationSpeed = 5f;
        public float minDistanceActivation = 3f;
        public Transform player;
        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip charge;
        [SerializeField] private AudioClip growl;
        

        private bool isCharging = false;
        private bool isPreparingCharge = false;
        private float rotationTimer = 0f;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D rb;
        private Vector2 currentDirection = Vector2.left;
        private bool hit = false;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (player == null)
            {
                Debug.LogWarning("Player reference is missing!");
                return;
            }
            
            if (Mathf.Abs(transform.position.x -player.position.x) > 1)
            {
                FlipSprite(currentDirection);
            } else if (Mathf.Abs(transform.position.x - player.position.x) < -1)
            {
                FlipSprite(currentDirection);
            }

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
    
            if (distanceToPlayer > minDistanceActivation && distanceToPlayer < detectionRange && !isCharging &&
                !isPreparingCharge && player.transform.position.y < transform.position.y  +0.5f && !hit)
            {
                print($"preparing charge since distance is {distanceToPlayer} and detection range is {detectionRange}");
                StartCoroutine(PrepareCharge());
            }
        }

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
            StartCoroutine(ChargeCooldown());
        }

        IEnumerator ChargeCooldown()
        {
            yield return new WaitForSeconds(chargeCooldown);
            isCharging = false;
        }

        void FixedUpdate()
        {
            currentDirection = (player.position - transform.position).x > 0 ? Vector2.right : Vector2.left;

            if (isCharging)
            {
                transform.position += (Vector3)currentDirection * (chargeSpeed * Time.fixedDeltaTime);
                RotateEnemy();
                if (Math.Abs(player.position.x - transform.position.x) < 1)
                {
                    isCharging = false;
                    src.Stop();
                }
            }
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

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.GetComponent<PlayerMovement>() is not null)
            {
                if (player.position.y < transform.position.y)
                {
                    // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }

        public void Growl()
        {
            src.clip = growl;
            src.Play();
            
        }

        public override void OnRam()
        {
            isCharging = false;
            hit = true;
            src.Stop();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(Vector2.right * 200f);
        }
    }
}