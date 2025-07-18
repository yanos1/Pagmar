using FMODUnity;
using FMOD.Studio;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Terrain.Environment
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class Box : MonoBehaviour, IResettable, IBreakable
    {
        private Rigidbody2D rb;
        private BoxCollider2D col;

        private Vector3 startingPosition;
        private bool isDropping = false;
        private bool dropTriggered = false;

        [Header("Drop Detection Settings")]
        [SerializeField] private float dropVelocityThreshold = -4f;
        [SerializeField] private float extraRaycastDistance = 0.2f;
        [SerializeField] private LayerMask groundLayers;

        [Header("FMOD Events")]
        [SerializeField] private EventReference boxDropEvent;
        [SerializeField] private EventReference boxPushEvent;
        [SerializeField] private EventReference boxBreakSound;

        [Header("Break Effects")]
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private MMF_Player breakFeedbacks;
        [SerializeField] private bool IsDymanic = true;
        private EventInstance pushInstance;
        private bool isPushSoundPlaying = false;

        private float hitForce = 20f;
        private float hitCooldownTimer = 0f;
        private const float hitCooldownDuration = 0.5f;
        private bool isMoving;
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<BoxCollider2D>();
            startingPosition = transform.position;

            if (!boxPushEvent.IsNull)
            {
                pushInstance = CoreManager.Instance.AudioManager.CreateEventInstance(boxPushEvent);
            }
        }
        
        private void Start()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        private void OnDestroy()
        {
            pushInstance.release();
        }

        private void FixedUpdate()
        {
            float verticalVelocity = rb.linearVelocity.y;
            bool isFalling = verticalVelocity < dropVelocityThreshold;

            Vector2 rayOrigin = (Vector2)transform.position + col.offset;
            float castDistance = (col.size.y * 0.5f * transform.localScale.y) + extraRaycastDistance;
            Vector2 down = -transform.up;

            bool isTouchingGround = Physics2D.Raycast(rayOrigin, down, castDistance, groundLayers);

            if (isFalling)
            {
                isDropping = true;
                dropTriggered = false;
            }

            if (isDropping && !dropTriggered && isTouchingGround && Mathf.Abs(verticalVelocity) < 0.1f)
            {
                isDropping = false;
                dropTriggered = true;
                        
                CoreManager.Instance.AudioManager.PlayOneShot(boxBreakSound, CoreManager.Instance.Player.transform.position) ;
                Debug.Log("Box dropped and landed.");
            }
        }

        private void Update()
        {
            if (hitCooldownTimer > 0f)
                hitCooldownTimer -= Time.deltaTime;

            if (isMoving && rb.linearVelocity.magnitude < 0.1f)
            {
                isMoving = false;
                StopPushSound();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();

            if (player != null)
            {
                Vector2 hitDir = (transform.position - player.transform.position).normalized;

                if (player.IsDashing)
                {
                    // Add dash hit logic if needed
                }
                else
                {
                    PlayPushSound();
                    isMoving = true;
                }
            }
        }

        public void OnBreak()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(boxBreakSound, CoreManager.Instance.Player.transform.position);

            if (e != null && f != null)
            {
                breakFeedbacks?.PlayFeedbacks();
                e.explode();
                f.doExplosion(transform.position);
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                
            }
        }

        public void OnHit(Vector2 hitDirection, PlayerStage stage)
        {
            print("box is hit!");
            if (stage == PlayerStage.FinalForm)
            {
                OnBreak();
            }
            else
            {
                PlayPushSound();
                isMoving = true;
                rb.AddForce(hitDirection * hitForce, ForceMode2D.Impulse);
            }
        }

        public void ResetToInitialState()
        {
            if (IsDymanic)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            } 
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.position = startingPosition; // Use this instead of transform.position
            rb.rotation = 0f;
            
            hitCooldownTimer = 0f;
            isDropping = false;
            dropTriggered = false;
            print("reset box");

            StopPushSound();
        }


        private void PlayPushSound()
        {
            if (!isPushSoundPlaying)
            {
                pushInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                pushInstance.start();
                isPushSoundPlaying = true;
            }
        }

        private void StopPushSound()
        {
            if (isPushSoundPlaying)
            {
                pushInstance.stop(STOP_MODE.IMMEDIATE);
                isPushSoundPlaying = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            if (col == null) col = GetComponent<BoxCollider2D>();
            Vector2 rayOrigin = (Vector2)transform.position + col.offset;
            float castDistance = (col.size.y * 0.5f * transform.localScale.y) + extraRaycastDistance;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(rayOrigin, rayOrigin + (Vector2)(-transform.up * castDistance));
        }
    }
}
