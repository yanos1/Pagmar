using System.Collections;
using Obstacles.Shooters.Projectiles;
using UnityEngine.Serialization;

namespace Obstacles.Shooters
{
    using UnityEngine;

    public class Shooter : MonoBehaviour, IShooter
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Vector2 direction;
        [SerializeField] private float force;
        private SpriteRenderer renderer;

        private void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            if (direction == Vector2.right)
            {
                renderer.flipX = true;
            }
        }

        public void Shoot()
        {
            Projectile projectile = Instantiate(bulletPrefab, firePoint ? firePoint.position : transform.position, Quaternion.identity)
                .GetComponent<Projectile>();
            projectile.Activate(direction, force);
        }

        private IEnumerator ChangeSpriteForShortDuration()
        {
            yield break;
        }
    }
}