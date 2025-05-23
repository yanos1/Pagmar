using System;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class BreakableLog : MonoBehaviour, IResettable
    {
        private int hitCount = 0;
        [SerializeField]private int maxHits = 7;
        private Vector3 startingPos;
        private Rigidbody2D rb;

        [SerializeField] private MMF_Player hitFeedback;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {   
            if (collision.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing && Mathf.Abs(player.DashDirection.y) > 0.3f)
            {
                print("hit tree 86");
                hitFeedback?.PlayFeedbacks();
                if (++hitCount == maxHits)
                {
                    Explode();
                }
            }
        }

        private void Explode()
        {
            e.explode();
            f.doExplosion(transform.position);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                e.explode();
            }
        }


        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            hitCount = 0;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = startingPos;
            transform.rotation = Quaternion.identity;
        }
    }
}