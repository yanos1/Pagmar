using System;
using Interfaces;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Triggers
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class FeedbackPlayer : MonoBehaviour, IResettable
    {
        [SerializeField] private MMF_Player feedbacks;
        private bool triggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggered && other.CompareTag("Player"))
            {
                triggered = true;
                feedbacks?.PlayFeedbacks();
            }
        }

        public void ResetToInitialState()
        {
            triggered = false;
        }
    }
}