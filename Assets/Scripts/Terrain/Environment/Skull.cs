using Interfaces;
using UnityEngine;

namespace Terrain.Environment
{
    public class Skull : MonoBehaviour, IResettable
    {
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private int hitThreshold = 100;

        private int hitCount = 0;
        private bool hasExploded = false;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        private void Start()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (hasExploded) return;
            print($"hit skulls times: {hitCount}");
            hitCount++;

            if (hitCount >= hitThreshold)
            {
                hasExploded = true;
                if (e != null) e.explode();
                if (f != null) f.doExplosion(f.transform.position);
            }
        }

        public void ResetToInitialState()
        {
            hitCount = 0;
            hasExploded = false;
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            // Optional: Reset Explodable and ExplosionForce state if needed
        }
    }
}