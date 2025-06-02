using System;
using System.Collections;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingGround : MonoBehaviour
    {
        [SerializeField] private MMF_Player breakFeedbacks;
        [SerializeField] private Explodable e;
        private Rigidbody2D rb;

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.PickUpFakeRune, OnPickUp);
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PickUpFakeRune, OnPickUp);
        }

        private void OnPickUp(object obj)
        {
            Shake();
        }

        private void Shake()
        {
            breakFeedbacks?.PlayFeedbacks();
        }

        
        // called from edidor (in feedbacks)
        public void Explode()
        {
            print("exlode ground");
            e.explode();
        }
    }
}