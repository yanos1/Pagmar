using System;
using FMODUnity;
using FMOD.Studio;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingStone : MonoBehaviour, IResettable, IKillPlayer
    {
        [SerializeField] private MMF_Player fallFeedbacks;
        [SerializeField] private MMF_Player landFeedBacks;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private bool canKillPlayer = true;
        [SerializeField] private bool resetSceneAfterDeath = false;
        [SerializeField] private EventReference rollSound;
        [SerializeField] private EventReference hitSound;

        protected Rigidbody2D rb;
        private Vector3 startingPos;

        private EventInstance rollSoundInstance;
        private bool isRollSoundPlaying = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startingPos = transform.position;
        }

        private void Update()
        {
            if (isRollSoundPlaying && rb.linearVelocity.y < -1f)
            {
                StopRollSound();
            }
        }

        public void Activate()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Create and start roll sound instance
            rollSoundInstance = CoreManager.Instance.AudioManager.CreateEventInstance(rollSound);
            RuntimeManager.AttachInstanceToGameObject(rollSoundInstance, gameObject, GetComponent<Rigidbody2D>());
            rollSoundInstance.start();
            isRollSoundPlaying = true;
        }

        private void StopRollSound()
        {
            if (isRollSoundPlaying)
            {
                rollSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                rollSoundInstance.release();
                isRollSoundPlaying = false;
            }
        }

        public virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("WeakRock") || other.gameObject.CompareTag("Metal") || other.gameObject.CompareTag("Player"))
            {
                // Play hit sound as one-shot
                CoreManager.Instance.AudioManager.PlayOneShot(hitSound, transform.position);

                if (e is not null)
                    e.explode();
                if (f is not null)
                    f.doExplosion(transform.position);
            }
        }

        public virtual void ResetToInitialState()
        {
            gameObject.SetActive(true);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = startingPos;
            transform.rotation = Quaternion.identity;

            StopRollSound();
        }

        public void HitPlayer()
        {
            // landFeedBacks?.PlayFeedbacks();
        }

        public virtual bool IsDeadly()
        {
            var deadly = rb.linearVelocity.y < 0 && canKillPlayer;
            if (deadly && resetSceneAfterDeath)
                ScenesManager.Instance.ReloadCurrentScene();
            return deadly;
        }
    }
}
