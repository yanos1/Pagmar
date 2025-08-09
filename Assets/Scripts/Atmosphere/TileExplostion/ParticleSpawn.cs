using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utility;

namespace Atmosphere.TileExplosion
{
    public class ParticleSpawn : Poolable
    {
        [Header("Particles")]
        [SerializeField] private List<ParticleSystem> explodingParticles;

        [Header("Pool Settings")]
        [SerializeField] private PoolEnum poolType;

        private Coroutine returnRoutine;
        private bool isReturning; // Prevents multiple returns

        private void Start()
        {
            Type = poolType;
        }

        public override void OnGetFromPool()
        {
            // Reset flags
            isReturning = false;

            // Stop any leftover coroutine from last use
            if (returnRoutine != null)
            {
                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
        }

        public void Play(Vector3 pos)
        {
            transform.position = pos;

            // Play all particles
            foreach (var ps in explodingParticles)
            {
                ps.Play(true);
            }

            // Start return coroutine
            returnRoutine = StartCoroutine(ReturnToPoolAfterDone());
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

            // Ensure we only return once
            if (!isReturning && this != null && gameObject != null)
            {
                isReturning = true;
                CoreManager.Instance.PoolManager.ReturnToPool(this);
            }
        }

        public override void OnReturnToPool()
        {
            // Ensure coroutines are cleaned up
            base.OnReturnToPool();
            if (returnRoutine != null)
            {
                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
        }
    }
}
