using System;
using System.Collections;
using Managers;
using UnityEngine;
using Utility;

namespace Atmosphere.TileExplostion
{
    public class ParticleSpawn : Poolable
    {

        [SerializeField] private ParticleSystem explodingParticles;
        [SerializeField] private PoolEnum poolType;
        private void Start()
        {
            Type = poolType;
        }

        public void Play(Vector3 pos)
        {
            transform.position = pos;
            explodingParticles.Play();
            StartCoroutine(ReturnToPoolAfterDone());
        }

        private IEnumerator ReturnToPoolAfterDone()
        {
            yield return new WaitUntil(() => explodingParticles.isPlaying == false);
            CoreManager.Instance.PoolManager.ReturnToPool(this);
        }
    }
}