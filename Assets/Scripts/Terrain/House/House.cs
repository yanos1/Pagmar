using System;
using Interfaces;
using MoreMountains.Feedbacks;
using SpongeScene;
using UnityEngine;

namespace Terrain.House
{
    public class House : MonoBehaviour, IResettable
    {
        private Vector3 startingPos;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private MMF_Player shakeFeedbacks;
        
        private void Start()
        {
            startingPos = transform.position;
            rb = GetComponent<Rigidbody2D>();
            shakeFeedbacks?.PlayFeedbacks();
        }

        public void ResetToInitialState()
        {
            transform.position = startingPos;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            shakeFeedbacks?.PlayFeedbacks();
        }

        public void OnEndShake()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}