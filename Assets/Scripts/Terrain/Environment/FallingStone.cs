using System;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingStone : MonoBehaviour, IResettable
    {
        [SerializeField] private float fallForce;
        [SerializeField] private MMF_Player fallFeedbacks;
        [SerializeField] private MMF_Player landFeedBacks;
        private Rigidbody2D rb;
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
                rb.AddForce(Vector2.down * fallForce);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // landFeedBacks?.PlayFeedbacks();
            }
        }

        public void ResetToInitialState()
        {
            transform.position = startingPos;
        }

        public void HitPlayer()
        {
            // landFeedBacks?.PlayFeedbacks();
        }
    }
}