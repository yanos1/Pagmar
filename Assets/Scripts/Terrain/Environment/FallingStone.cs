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
        protected Rigidbody2D rb;
        private Vector3 startingPos;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is { } player)
            {
                fallFeedbacks?.PlayFeedbacks();
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 1.5f;

            }
        }

        public virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // landFeedBacks?.PlayFeedbacks();
                gameObject.SetActive(false);
            }
        }

        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; 
            transform.position = startingPos;
        }

        public void HitPlayer()
        {
            // landFeedBacks?.PlayFeedbacks();
        }

        public virtual bool IsDeadly()
        {
            print("445 kill player");
            return true;
        }
    }
}