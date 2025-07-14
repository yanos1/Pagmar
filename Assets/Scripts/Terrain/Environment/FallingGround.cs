using System;
using System.Collections;
using FMODUnity;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;

namespace Terrain.Environment
{
    public class FallingGround : MonoBehaviour
    {
        [SerializeField] private MMF_Player breakFeedbacks;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private EventReference collapseSound;
        private Rigidbody2D rb;

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.PickUpFakeRune, OnPickUp);
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PickUpFakeRune, OnPickUp);
        }

        private void OnPickUp(object obj)
        {
            CoreManager.Instance.AudioManager.PlayOneShot(collapseSound, transform.position);
            Shake();
        }

        private void Shake()
        {
            breakFeedbacks?.PlayFeedbacks();
        }

        
        // called from edidor (in feedbacks)
        public void Explode()
        {
            print("exlode ground");
            e.explode();
            f.doExplosion(f.transform.position);
        }
    }
}