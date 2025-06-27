using System;
using Interfaces;
using Managers;
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
        [SerializeField] private bool resetSceneAfterDeath = false;
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
            if (other.gameObject.CompareTag("WeakRock") || other.gameObject.CompareTag("Metal") || other.gameObject.CompareTag("Player"))  // surfaces that collide with the stone.
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
            var deadly = rb.linearVelocity.y < 0 && canKillPlayer;
            if (deadly && resetSceneAfterDeath) ScenesManager.Instance.ReloadCurrentScene();
            return deadly;
        }
    }
}