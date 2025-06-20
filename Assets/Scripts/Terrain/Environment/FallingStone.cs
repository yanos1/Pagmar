using System;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingStone : MonoBehaviour, IResettable, IKillPlayer
    {
        [SerializeField] private MMF_Player fallFeedbacks;
        [SerializeField] private MMF_Player landFeedBacks;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private bool canKillPlayer = true;
        protected Rigidbody2D rb;
        private Vector3 startingPos;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        public void Activate()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        private System.Collections.IEnumerator ApplyTemporaryVelocity(Vector2 direction, float speed, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                rb.linearVelocity = direction * speed;
                timer += Time.deltaTime;
                yield return null;
            }

            // Optionally reset velocity if needed
        }

        public virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<MovingPlatform>() is not null)
            {
                if (e is not null)
                    e.explode();
                if (f is not null)
                    f.doExplosion(transform.position);
            }

        }

        public virtual void ResetToInitialState()
        {
            print("reset stone 77");
            gameObject.SetActive(true);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            // rb.bodyType = RigidbodyType2D.Kinematic; 
            transform.position = startingPos;
            transform.rotation = Quaternion.identity;
        }

        public void HitPlayer()
        {
            // landFeedBacks?.PlayFeedbacks();
        }

        public virtual bool IsDeadly()
        {
            print("445 kill player");
            return rb.linearVelocity.y < 0 && canKillPlayer;
        }
    }
}