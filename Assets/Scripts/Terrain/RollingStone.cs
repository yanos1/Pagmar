using System;
using Enemies;
using UnityEngine;

namespace Terrain
{
    public class RollingStone : MonoBehaviour
    {
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<ChargingEnemy>() is not null)
            {
                gameObject.layer = LayerMask.NameToLayer("EnemyIgnore");
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.left * 1000);

            }
        }
    }
}
