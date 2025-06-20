using System;
using Managers;
using Player;
using UnityEngine;
using EventReference = FMODUnity.EventReference;

namespace Triggers
{
    public class LavaDeathZone : MonoBehaviour
    {
        [SerializeField] private EventReference lavaSplash;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is { } player)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(lavaSplash, transform.position);
                player.Die();
            }
        }
    }
}