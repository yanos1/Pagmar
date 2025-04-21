using System.Collections;
using Managers;
using Obstacles.Shooters.Projectiles;
using UnityEngine.Serialization;

namespace Obstacles.Shooters
{
    using UnityEngine;

    public class Shooter : MonoBehaviour, IShooter
    {
        [SerializeField] private PoolEnum bulletType;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Vector2 direction;
        [SerializeField] private float force;
        private SpriteRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (direction == Vector2.right)
            {
                _renderer.flipX = true;
            }
        }

        public void Shoot()
        {
            Projectile projectile = CoreManager.Instance.PoolManager.GetFromPool<Projectile>(bulletType);
            projectile.Activate(transform.position,direction, force);
        }

        private IEnumerator ChangeSpriteForShortDuration()
        {
            yield break;
        }
    }
}