using System;
using Enemies;
using Interfaces;
using Managers;
using Player;
using Terrain;
using Terrain.Environment;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Obstacles.Shooters.Projectiles
{
    public class Projectile : MonoBehaviour, IResettable
    {
        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponent<SpriteRenderer>();
            CoreManager.Instance.ResetManager.AddResettable(this); //careful this is big prone!
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
           
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<Box>() || other.gameObject.GetComponent<Enemy>())
            {
                _rb.bodyType = RigidbodyType2D.Static;
                gameObject.transform.parent = other.transform;
            }
            

            if (other.gameObject.GetComponent<Projectile>() is not null)
            {
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

        public bool IsDeadlyProjectile()
        {
            return _rb.bodyType == RigidbodyType2D.Dynamic;
        }

        public void ResetToInitialState()
        {
            Destroy(this.gameObject);
        }
    }
}