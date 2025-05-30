using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.House
{
    public class FallingHouseObject: MonoBehaviour
    {
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        private Rigidbody2D rb;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<ExplodingHouseObject>() is { } explodingHouseObject)
            {
                e.explode();
                f.doExplosion(f.transform.position);
                explodingHouseObject.Explode();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}