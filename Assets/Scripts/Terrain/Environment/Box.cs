using System;
using Interfaces;
using Managers;
using Obstacles;
using Player;
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

        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip boxHit;
        [SerializeField] private AudioClip boxPush;
        [SerializeField] private AudioClip boxDrop;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        private float hitCooldownTimer = 0f;
        private const float hitCooldownDuration = 0.5f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPosition = transform.position;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement playerMovement2 = other.gameObject.GetComponent<PlayerMovement>();

            if (playerMovement2 != null)
            {
                Vector2 hitDirection = (transform.position - other.transform.position).normalized;

                if (playerMovement2.IsDashing)
                {
                    if (hitCooldownTimer <= 0f)
                    {
                        PlaySound(boxHit);
                        OnHit(hitDirection, CoreManager.Instance.Player.playerStage);
                        hitCooldownTimer = hitCooldownDuration;
                    }
                }
                else // not dashing
                {
                    PlaySound(boxPush, loop: true);
                    isMoving = true;
                }
            }

            if ((other.gameObject.GetComponent<GuillotineTrap>() is { } guil && guil.IsDeadly()))
            {
                e.explode();
                f.doExplosion(transform.position);
                print("explode");
            }
        }

        public void OnBreak()
        {
            e.explode();
            f.doExplosion(transform.position);
        }

        public void OnHit(Vector2 hitDirection, PlayerStage stage)
        {
            print("box hit 88");
            if (stage == PlayerStage.Adult)
            {
                print("box break 88");
                OnBreak();
            }
            else{
                
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
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            transform.position = startingPosition;
            hitCooldownTimer = 0;
        }
    }
}
