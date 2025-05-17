using System;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class BouncyLog : MonoBehaviour, IResettable
    {
        private Rigidbody2D rb;
        private Rigidbody2D playerRb;
        private Vector3 startingPos;
        
        [SerializeField] private float force;
        [SerializeField] private MMF_Player hitFeedbacks;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<PlayerMovement>() is { } player)
            {
                playerRb = player.GetComponent<Rigidbody2D>();
            }
        }

        

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<FallingStone>() is not null)
            {
                hitFeedbacks?.PlayFeedbacks();
                Invoke(nameof(AddForceToPlayer), 0.1f );
            }
        }

        public void AddForceToPlayer()
        {
            playerRb.AddForce(Vector2.up * force);
        }

        public void ResetToInitialState()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.rotation = Quaternion.identity;
            transform.position = startingPos;
        }
    }
}