﻿using Interfaces;

namespace Terrain.Environment
{
    using UnityEngine;
    using DG.Tweening; // Add DoTween namespace

    public class CrumblingPlatform : MonoBehaviour, IResettable
    {
        public float crumbleDelay = 2f;
        public bool destroyAfterCrumble = false;
        public float crumbleDuration = 0.7f; // Duration of the crumble effect
        public Animator animator; // Optional for crumble animation

        private bool hasCrumbled = false;
        private Collider2D col;
        private SpriteRenderer sr;
        private Vector3 initialPosition;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();

            initialPosition = transform.position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hasCrumbled) return;
            
            if (collision.gameObject.GetComponent<PlayerMovement>() is not null && collision.gameObject.transform.position.y > transform.position.y)
            {
                hasCrumbled = true;
                Invoke(nameof(Crumble), crumbleDelay);
                Invoke(nameof(ResetToInitialState),5);

            }

        }

        private void Crumble()
        {
            // if (animator != null)
            //     animator.SetTrigger("Crumble");

            transform.DOMoveY(transform.position.y - 2f, crumbleDuration) 
                .SetEase(Ease.InQuad);
            sr.DOFade(0f, crumbleDuration).OnComplete(DisablePlatform);
            col.enabled = false;
            
        }

        private void DisablePlatform()
        {
            col.enabled = false;
        }

        public virtual void ResetToInitialState()
        {
            
            hasCrumbled = false;
            transform.position = initialPosition;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            col.enabled = true;
        }
    }
}