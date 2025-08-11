using System;
using Atmosphere.TileExplosion;
using Atmosphere.TileExplostion;
using Managers;
using Player;
using Terrain.Environment;
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
                CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.LavaSplashParticles).Play(other.transform.position);
                player.Die();
            }
            else if (other.GetComponent<Box>() is { } box)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(lavaSplash, transform.position);
                CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.LavaSplashParticles).Play(other.transform.position);
            }
        }
    }
}