using System;
using Managers;
using Player;
using UnityEngine;

namespace Triggers
{
        public class DeathZone : MonoBehaviour
        {
            private void OnTriggerEnter2D(Collider2D other)
            {
                if (other.GetComponent<PlayerManager>() is { } player)
                {
                    print("enter death zone");
                    player.Die();
                }
            }
        }
    }
