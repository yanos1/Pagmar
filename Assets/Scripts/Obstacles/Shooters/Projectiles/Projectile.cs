using System;
using Player;
using Terrain;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Obstacles.Shooters.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
           
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<BreakableTerrain>())
            {
                _rb.bodyType = RigidbodyType2D.Static;
            }
            
            if (other.gameObject.GetComponent<PlayerMovement>() is not null || other.gameObject.GetComponent<PlayerMovement>() is not null)
            {
                if (_rb.bodyType == RigidbodyType2D.Dynamic)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }

            if (other.gameObject.GetComponent<Projectile>() is not null)
            {
                return;
            }
        }

        public void Activate(Vector3 bulletDirection, float bulletForce)
        {
            if (bulletDirection == Vector3.right)
            {
                // renderer.flipX = true;
            }
            else if (bulletDirection == Vector3.down)
            {
                transform.Rotate(0, 0, -90);
            }

            _rb.AddForce(bulletDirection * bulletForce);
        }
    }
}