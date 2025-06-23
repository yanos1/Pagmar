using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Atmosphere.TileExplostion
{
    public class ParticleSpawn : Poolable
    {
        [Header("Particles")] [SerializeField] private List<ParticleSystem> explodingParticles;

        [Header("Pool Settings")]
        [SerializeField] private PoolEnum poolType;

        private void Start()
        {
            Type = poolType;
        }

        public void Play(Vector3 pos)
        {
            transform.position = pos;

            foreach (var ps in explodingParticles)
            {
                ps.Play();
            }

            StartCoroutine(ReturnToPoolAfterDone());
        }

        private IEnumerator ReturnToPoolAfterDone()
        {
            // Wait until all particles are done playing
            yield return new WaitUntil(() =>
            {
                foreach (var ps in explodingParticles)
                {
                    if (ps.isPlaying)
                        return false;
                }
                return true;
            });

            CoreManager.Instance.PoolManager.ReturnToPool(this);
        }
    }
}