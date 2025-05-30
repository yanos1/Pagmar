using System;
using Managers;
using UnityEngine;

namespace Triggers
{
    public class IncreaseShakeTrigger : MonoBehaviour
    {
        [SerializeField] private CollapsingRoomManager _roomManager;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _roomManager.InvokeNextFeedbacks();
            }
        }
    }
}