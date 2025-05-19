using System;
using Interfaces;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class BreakableLog : MonoBehaviour, IResettable
    {
        private int hitCount = 0;
        private const int maxHits = 5;
        private Vector3 startingPos;
        private Rigidbody2D rb;
        private bool firstHit = true;

        [SerializeField] private MMF_Player hitFeedback;
        [SerializeField] private Explodable e;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.gameObject.GetComponent<PlayerMovement>() is { } player && (player.IsDashing || firstHit))
            {
                firstHit = false;
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
            firstHit = true;
            hitCount = 0;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = startingPos;
            transform.rotation = Quaternion.identity;
        }
    }
}