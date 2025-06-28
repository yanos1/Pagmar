using Atmosphere;
using Managers;
using ScripableObjects;
using UnityEngine;

namespace Triggers
{
    public class MirrorCheckpoint : Checkpoint
    {
        [SerializeField] private ParticleAttractor _particleAttractor;
        [SerializeField] private MirrorCheckpointSounds sounds;
        public override void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggered)
            {
                base.OnTriggerEnter2D(other);
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.reachedMirrorSound, transform.position);
                triggered = true;
            }
       
        }

        public void PullParticles()
        {
            _particleAttractor.StartAttraction();
        }
    }
}