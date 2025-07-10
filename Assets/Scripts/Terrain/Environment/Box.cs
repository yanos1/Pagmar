using FMODUnity;
using FMOD.Studio;
using Interfaces;
using Managers;
using UnityEngine;

namespace Terrain.Environment
{
    public class Box : MonoBehaviour, IResettable, IBreakable
    {
        private Rigidbody2D rb;
        private float hitForce = 20; // this is a dummy value that will be obtained from the player.
        private bool isMoving;
        private bool isDropping;
        private Vector3 startingPosition;

        [SerializeField] private EventReference boxHitEvent;
        [SerializeField] private EventReference boxPushEvent;
        [SerializeField] private EventReference boxDropEvent;
        [SerializeField] private EventReference boxBreakSound;

        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;

        private EventInstance pushInstance;
        private bool isPushSoundPlaying = false;

        private float hitCooldownTimer = 0f;
        private const float hitCooldownDuration = 0.5f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPosition = transform.position;

            // Create push sound instance
            if (boxPushEvent.IsNull == false)
            {
                pushInstance = CoreManager.Instance.AudioManager.CreateEventInstance(boxPushEvent);
            }
        }

        private void OnDestroy()
        {
            pushInstance.release();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement playerMovement2 = other.gameObject.GetComponent<PlayerMovement>();

            if (playerMovement2 != null)
            {
                Vector2 hitDirection = (transform.position - other.transform.position).normalized;

                if (playerMovement2.IsDashing)
                {
                    // Uncomment this if you want dash-hit logic
                    // if (hitCooldownTimer <= 0f)
                    // {
                    //     CoreManager.Instance.AudioManager.PlayOneShot(boxHitEvent, transform.position);
                    //     OnHit(hitDirection, CoreManager.Instance.Player.playerStage);
                    //     hitCooldownTimer = hitCooldownDuration;
                    // }
                }
                else // Not dashing, so just pushing
                {
                    PlayPushSound();
                    isMoving = true;
                }
            }
        }

        public void OnBreak()
        {
            if (e != null && f != null)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(boxBreakSound, transform.position);
                e.explode();
                f.doExplosion(transform.position);
            }
        }

        public void OnHit(Vector2 hitDirection, PlayerStage stage)
        {
            if (stage == PlayerStage.FinalForm)
            {
                OnBreak();
            }
            else
            {
                rb.AddForce(hitDirection * hitForce, ForceMode2D.Impulse);
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

        private void FixedUpdate()
        {
            if (Mathf.Approximately(rb.linearVelocity.y, 0) && isDropping)
            {
                isDropping = false;
                CoreManager.Instance.AudioManager.PlayOneShot(boxDropEvent, transform.position);
            }

            if (rb.linearVelocity.y < -0.1f)
            {
                isDropping = true;
            }
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
                pushInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                isPushSoundPlaying = false;
            }
        }

        public void ResetToInitialState()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = startingPosition;
            hitCooldownTimer = 0f;

            StopPushSound();
        }
    }
}