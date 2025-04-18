using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class Box : MonoBehaviour, IResettable, IBreakable
    {
        private Rigidbody2D rb;
        private float hitForce = 40f; // this is a dummy value that will be obtained from the player.
        private bool isMoving;
        private bool isDropping;
        private Vector3 startingPosition;

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip boxHit;
        [SerializeField] private AudioClip boxPush;
        [SerializeField] private AudioClip boxDrop;
        

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPosition = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement playerMovement2 = other.gameObject.GetComponent<PlayerMovement>();

            if (playerMovement2 != null || playerMovement2 != null)
            {
                Vector2 hitDirection = (transform.position - other.transform.position).normalized;

                if (playerMovement2 != null && playerMovement2.IsDashing) // && player.isBig => then break
                {
                    PlaySound(boxHit);
                    OnHit(hitDirection);

                    // OnBreak();
                }
                else if (playerMovement2 != null && !playerMovement2.IsDashing)  // && player.isSmall => move it
                {
                    PlaySound(boxPush, loop: true);
                    isMoving = true;
                }
            }
        }

        public void OnBreak()
        {
            gameObject.SetActive(false);
        }

        public void OnHit(Vector2 hitDirection)
        {
            if (rb != null)
            {
                rb.AddForce(hitDirection * hitForce, ForceMode2D.Impulse);
            }
        }

        private void Update()
        {
            if (isMoving && rb.linearVelocity.magnitude < 0.1f)
            {
                isMoving = false;
                StopSound();
            }
        }

        private void PlaySound(AudioClip clip, bool loop = false)
        {
            if (src != null && clip != null)
            {
                src.clip = clip;
                src.loop = loop;
                src.Play();
            }
        }

        private void StopSound()
        {
            if (src != null && src.isPlaying)
            {
                src.loop = false;
                src.Stop();
            }
        }

        private void FixedUpdate()
        {
            if (Mathf.Approximately(rb.linearVelocity.y, 0) && isDropping)
            {
                isDropping = false;
                PlaySound(boxDrop);
            }
            if (rb.linearVelocity.y < -0.1f)
            {
                isDropping = true;
                
            }
        }

        public void ResetToInitialState()
        {
            transform.position = startingPosition;
        }
    }
}
