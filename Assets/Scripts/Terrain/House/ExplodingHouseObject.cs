using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.House
{
    public class ExplodingHouseObject : MonoBehaviour
    {
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Explode();
            }
        }

        public void Explode()
        {
            e.explode();
            f.doExplosion(f.transform.position);
        }
    }
}