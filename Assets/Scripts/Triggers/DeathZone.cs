using System;
using Managers;
using UnityEngine;

namespace Triggers
{
        public class DeathZone : MonoBehaviour
        {
            private void OnTriggerEnter2D(Collider2D other)
            {
                if (other.GetComponent<PlayerMovement>() is { } player)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
                }
            }
        }
    }
