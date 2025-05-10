using System;
using Interfaces;
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
                rb.bodyType = RigidbodyType2D.Dynamic;
                
                Invoke(nameof(AddForceToPlayer), 0.1f );
            }
        }

        public void AddForceToPlayer()
        {
            playerRb.AddForce(Vector2.up * force);
        }

        public void ResetToInitialState()
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.position = startingPos;
            transform.rotation = Quaternion.identity;
        }
    }
}