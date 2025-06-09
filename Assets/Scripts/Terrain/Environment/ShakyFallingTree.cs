using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class ShakyFallingTree : MonoBehaviour, IBreakable, IResettable
    {
        public int hitThreshold = 3;
        public float fallForce = 30f;
        public float hitCooldown = 0.5f; // seconds between valid hits
        
        // New particle hit threshold and counter
        public int particleHitThreshold = 200;
        private int particleHitCount = 0;

        private int hitCount = 0;
        private bool hasFallen = false;
        private bool canBeHit = true;
        private Vector3 startingPos;

        private Rigidbody2D rb2D;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private Explodable e;
        [SerializeField] private List<CrumblingPlatform> branches;

        void Start()
        {
            startingPos = transform.position;
            rb2D = GetComponent<Rigidbody2D>();
            if (rb2D != null)
            {
                rb2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (!canBeHit) return;

            if (collision.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                hitCount++;
                hitFeedbacks?.PlayFeedbacks();
                canBeHit = false;
                StartCoroutine(HitCooldownCoroutine());

                if (hitCount >= hitThreshold)
                {
                    FallRight();
                }
            }
        }

        private System.Collections.IEnumerator HitCooldownCoroutine()
        {
            yield return new WaitForSeconds(hitCooldown);
            canBeHit = true;
        }

        void FallRight()
        {
            hasFallen = true;
            foreach (var branch in branches)
            {
                branch.CrumbleQuick();
            }
            if (rb2D != null)
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                rb2D.AddForce(Vector2.right * fallForce, ForceMode2D.Impulse);
            }
        }

        // Particle collision counting here:
        void OnParticleCollision(GameObject other)
        {
            if (hasFallen) return;  // Already fallen, no need to count further

            particleHitCount++;

            if (particleHitCount >= particleHitThreshold)
            {
                OnBreak();
                hasFallen = true; // prevent multiple breaks
            }
        }

        public void OnBreak()
        {
            foreach (var branch in branches)
            {
                branch.CrumbleQuick();
            }
            e.explode();
            f.doExplosion(f.transform.position);
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            return;
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            transform.position = startingPos;
            rb2D.angularVelocity = 0;
            transform.rotation = Quaternion.identity;
            rb2D.bodyType = RigidbodyType2D.Kinematic;
            
        }
    }
}
