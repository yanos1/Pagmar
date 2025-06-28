using Managers;
using ScripableObjects;
using UnityEngine;

namespace Triggers
{
    public class MirrorCheckpoint : Checkpoint
    {
        [SerializeField] private MirrorCheckpointSounds sounds;
        public override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.reachedMirrorSound, transform.position);
        }
    }
}