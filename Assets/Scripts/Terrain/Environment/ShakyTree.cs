using System;
using Interfaces;
using MoreMountains.Feedbacks;
using SpongeScene;
using UnityEngine;
using UnityEngine.Serialization;

namespace Terrain.Environment
{
    public class ShakyTree : MonoBehaviour, IResettable
    {

        private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D branchCollider;
        [SerializeField] private MMF_Player hitFeedbacks;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<PlayerMovement>() is { } player && player.IsDashing)
            {
                hitFeedbacks?.PlayFeedbacks();
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.5f, () => branchCollider.enabled = false));
            }
        }

        public void ResetToInitialState()
        {
            branchCollider.enabled = true;
        }
    }
}