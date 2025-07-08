using System;
using Interfaces;
using Managers;
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
            shakeFeedbacks?.StopFeedbacks();
            transform.position = startingPos;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if (Vector3.Distance(CoreManager.Instance.Player.transform.position, transform.position) <
                25) // we are near house. 
            {
                shakeFeedbacks?.PlayFeedbacks();
            }
        }

        public void OnEndShake()
        {
            if (CoreManager.Instance.Player.IsDead) return; // avoids a bug
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}