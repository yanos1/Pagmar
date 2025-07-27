using FMODUnity;
using FMOD.Studio;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
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
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.3f, () => // just a wierd bug no time to fix so a plaster
            {
                transform.position = startingPosition;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
            }));
        }

        private void OnDestroy()
        {
            pushInstance.release();
        }

        private void FixedUpdate()
        {
            float verticalVelocity = rb.linearVelocity.y;
            bool isFalling = verticalVelocity < dropVelocityThreshold;

            if (isFalling)
            {
                isDropping = true;
                dropTriggered = false;
            }

            if (isDropping && !dropTriggered && IsTouchingGroundAnySide() && rb.linearVelocity.magnitude < 0.1f)
            {
                isDropping = false;
                dropTriggered = true;

                CoreManager.Instance.AudioManager.PlayOneShot(boxDropEvent, transform.position);
                Debug.Log("Box dropped and landed on any side.");
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

            if (isPushSoundPlaying)
            {
                pushInstance.set3DAttributes(RuntimeUtils.To3DAttributes(CoreManager.Instance.Player.transform.position));
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();

            if (player != null)
            {
                if (!player.IsDashing)
                {
                    PlayPushSound();
                    isMoving = true;
                }
            }
        }

        public void OnBreak()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(boxBreakSound, transform.position);

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

            rb.position = startingPosition;
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
                pushInstance.set3DAttributes(RuntimeUtils.To3DAttributes(CoreManager.Instance.Player.transform.position));
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

        private bool IsTouchingGroundAnySide()
        {
            Vector2 center = (Vector2)transform.position + col.offset;
            float halfWidth = col.size.x * 0.5f * transform.localScale.x;
            float halfHeight = col.size.y * 0.5f * transform.localScale.y;

            Vector2[] directions = {
                Vector2.down,
                Vector2.up,
                Vector2.left,
                Vector2.right
            };

            Vector2[] origins = {
                center + Vector2.down * halfHeight,
                center + Vector2.up * halfHeight,
                center + Vector2.left * halfWidth,
                center + Vector2.right * halfWidth
            };

            for (int i = 0; i < directions.Length; i++)
            {
                if (Physics2D.Raycast(origins[i], directions[i], extraRaycastDistance, groundLayers))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            if (col == null) col = GetComponent<BoxCollider2D>();

            Vector2 center = (Vector2)transform.position + col.offset;
            float halfWidth = col.size.x * 0.5f * transform.localScale.x;
            float halfHeight = col.size.y * 0.5f * transform.localScale.y;

            Vector2[] directions = {
                Vector2.down,
                Vector2.up,
                Vector2.left,
                Vector2.right
            };

            Vector2[] origins = {
                center + Vector2.down * halfHeight,
                center + Vector2.up * halfHeight,
                center + Vector2.left * halfWidth,
                center + Vector2.right * halfWidth
            };

            Gizmos.color = Color.magenta;

            for (int i = 0; i < directions.Length; i++)
            {
                Gizmos.DrawLine(origins[i], origins[i] + directions[i] * extraRaycastDistance);
            }
        }
    }
}
