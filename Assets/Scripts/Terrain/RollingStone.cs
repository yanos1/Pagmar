using System;
using Enemies;
using UnityEngine;

namespace Terrain
{
    public class RollingStone : MonoBehaviour
    {
        private Rigidbody2D rb;
        [SerializeField] private AudioSource src;
        [SerializeField] private AudioClip roll;
        [SerializeField] private AudioClip hit;

        private bool fell = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            ChargingEnemy enemy = other.gameObject.GetComponent<ChargingEnemy>();
            if (enemy is not null)
            {
                src.Stop();
                src.clip = hit;
                src.Play();
                enemy.Growl();
                gameObject.layer = LayerMask.NameToLayer("EnemyIgnore");
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.left * 1000);
            }

            PlayerMovement2 player = other.gameObject.GetComponent<PlayerMovement2>();
            if(player is not null && !fell)
            {
                src.clip = roll;
                src.Play();
                fell = true;
            }
        }
    }
}
