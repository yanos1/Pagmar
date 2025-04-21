using System;
using Enemies;
using Interfaces;
using Managers;
using Player;
using Terrain;
using Terrain.Environment;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Obstacles.Shooters.Projectiles
{
    public class Projectile : Poolable, IResettable
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
            if (other.gameObject.GetComponent<Box>() || other.gameObject.GetComponent<Enemy>())
            {
                _rb.bodyType = RigidbodyType2D.Static;
                gameObject.transform.parent = other.transform;
            }
            

            if (other.gameObject.GetComponent<Projectile>() is not null)
            {
            }
        }

        public void Activate(Vector3 pos, Vector3 bulletDirection, float bulletForce)
        {
            transform.position = pos;
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
            CoreManager.Instance.PoolManager.ReturnToPool(this);
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            transform.rotation = Quaternion.identity;
        }
    }
}