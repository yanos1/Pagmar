using SpongeScene;

namespace Atmosphere
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ParticleAttractor : MonoBehaviour
    {
        public ParticleSystem particleSystem;
        public Transform target;
        public float attractionDuration = 2f;

        private ParticleSystem.Particle[] particles;
        private bool attract = false;
        private float timer = 0f;

        // Store initial positions of each particle when attraction begins
        private List<Vector3> initialPositions = new List<Vector3>();

        void LateUpdate()
        {
            if (!attract) return;

            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / attractionDuration);

            int aliveCount = particleSystem.particleCount;

            if (particles == null || particles.Length < aliveCount)
                particles = new ParticleSystem.Particle[aliveCount];

            int count = particleSystem.GetParticles(particles);

            Vector3 targetPos = target.position - transform.position + new Vector3(-0.3f,1,0); // global target

            for (int i = 0; i < count; i++)
            {
                // If particle died or wasn't tracked, skip
                if (i >= initialPositions.Count)
                    continue;

                Vector3 startPos = initialPositions[i];
                particles[i].position = Vector3.Lerp(startPos, targetPos, t);
            }

            particleSystem.SetParticles(particles, count);

            if (timer >= attractionDuration)
            {
                attract = false;
                particleSystem.Stop(); // let current particles fade
            }
        }

        public void StartAttraction()
        {
            attract = true;
            timer = 0f;
            initialPositions.Clear();

            int aliveCount = particleSystem.particleCount;
            if (particles == null || particles.Length < aliveCount)
                particles = new ParticleSystem.Particle[aliveCount];

            int count = particleSystem.GetParticles(particles);

            for (int i = 0; i < count; i++)
                initialPositions.Add(particles[i].position);

            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
