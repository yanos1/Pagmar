using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.House
{
    public class Picture : MonoBehaviour
    {
        [SerializeField] private MMF_Player collapseFeedbacks;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                collapseFeedbacks.PlayFeedbacks();
            }
        }
    }
}