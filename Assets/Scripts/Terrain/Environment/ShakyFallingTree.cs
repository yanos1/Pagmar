using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class ShakyFallingTree : MonoBehaviour
    {
        public int hitThreshold = 3;
        public float fallForce = 30f;
        public float hitCooldown = 0.5f; // seconds between valid hits

        private int hitCount = 0;
        private bool hasFallen = false;
        private bool canBeHit = true;

        private Rigidbody2D rb2D;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private ExplosionForce f;

        void Start()
        {
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
            
            else if (collision.gameObject.GetComponent<OneTimeBeam>() is not null)
            {
                print("collided with one time beam 90");
                // f.doExplosion(f.transform.position);
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

            if (rb2D != null)
            {
                rb2D.bodyType = RigidbodyType2D.Dynamic;
                rb2D.AddForce(Vector2.right * fallForce, ForceMode2D.Impulse);
            }
        }
    }
}