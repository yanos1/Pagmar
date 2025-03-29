using System;
using Player;
using UnityEngine;

namespace Terrain
{
    public class BreakableTerrain : MonoBehaviour, IBreakable
    {
        private Rigidbody2D rb;
        private float hitFroce = 40f; // this is a dummy value that will be obtained from the player.


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            PlayerMovement2 playerMovement2 = other.gameObject.GetComponent<PlayerMovement2>();

            if (playerMovement != null || playerMovement2 != null)
            {
                Vector2 hitDirection = (transform.position - other.transform.position).normalized;

                if (playerMovement != null && playerMovement.IsDashing) // && player.isBig => then break
                {
                    // OnBreak();
                }
                
                if (playerMovement != null && playerMovement.IsDashing)  // && player.isSmall => move it
                {
                    OnHit(hitDirection);
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
                rb.AddForce(hitDirection * hitFroce, ForceMode2D.Impulse);
            }
        }
    }
}