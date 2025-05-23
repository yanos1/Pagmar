using System;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class Gate : MonoBehaviour, IResettable
    {
        private int hitCount = 0;
        [SerializeField]private int maxHits = 7;
        private Vector3 startingPos;
        private Rigidbody2D rb;

        [SerializeField] private MMF_Player hitFeedback;
        [SerializeField] private MMF_Player openFeedbacks;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {

            if (collision.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                hitFeedback?.PlayFeedbacks();
                if (++hitCount == maxHits)
                {
                    Explode();
                }
            }
        }

        public void Open()
        {
            openFeedbacks.PlayFeedbacks();
        }
        private void Explode()
        {
            if(e is null) return;
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
            transform.position = startingPos;
        }
    }
}