﻿using System;
using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Player;
using SpongeScene;
using UnityEngine;

namespace Terrain.Environment
{
    public class Hill : MonoBehaviour, IBreakable, IResettable
    {
        private Rigidbody2D _rb;
        private Vector3 startingPos;

        [SerializeField] private Rigidbody2D objectOnTop;
        [SerializeField] private float baseForce = 300f;
        [SerializeField] private float addedForce = 50;
        [SerializeField] private MMF_Player hitFeedbacks;
        [SerializeField] private MMF_Player breakFeedbacks;
        [SerializeField] private Vector2 dir = Vector2.up;
        [SerializeField] private Explodable e;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private float rayDistance = 0.5f;
        [SerializeField] private LayerMask objectOnTopLayer;
        [SerializeField] private int hitsToDestroy = 1;
        [SerializeField] private EventReference hitSound;
        [SerializeField] private EventReference breakSound;
        [SerializeField] private GameObject cracks;
        
        private bool isObjectOnTop = false;
        private float accumulatedForce;
        [SerializeField] private Collider2D _collider;
        private int currentHits;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            startingPos = transform.position;
            accumulatedForce = baseForce;
        }

        private void OnMouseDown()
        {
            OnHit(Vector2.right, PlayerStage.Young);
        }

        private void Update()
        {
            CheckObjectOnTop();
        }

        private void CheckObjectOnTop()
        {
            if (_collider == null) return;

            Bounds bounds = _collider.bounds;

            Vector2 topLeft = new Vector2(bounds.min.x, bounds.max.y +0.2f);
            Vector2 topRight = new Vector2(bounds.max.x, bounds.max.y + 0.2f);

            bool hitTop = false;
            bool hitRight = false;

            // Raycast upwards from top-left
            RaycastHit2D hit1 = Physics2D.Raycast(topLeft, Vector2.up, rayDistance, objectOnTopLayer);
            if (hit1.collider != null && hit1.rigidbody == objectOnTop)
            {
                hitTop = true;
            }

            // Raycast upwards from top-right
            RaycastHit2D hit2 = Physics2D.Raycast(topRight, Vector2.up, rayDistance, objectOnTopLayer);
            if (hit2.collider != null && hit2.rigidbody == objectOnTop)
            {
                hitRight = true;
            }

            bool wasOnTop = isObjectOnTop;
            isObjectOnTop = hitTop || hitRight;
            if (!isObjectOnTop && wasOnTop)
            {
                accumulatedForce = baseForce;
            }

            Debug.DrawRay(topLeft, Vector2.up * rayDistance, hitTop ? Color.green : Color.red);
            Debug.DrawRay(topRight, Vector2.up * rayDistance, hitRight ? Color.green : Color.red);
        }


        public void OnBreak()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(breakSound, transform.position);

            if (f is not null && e is not null)
            {
                print("play hill feedbacks");
                breakFeedbacks?.PlayFeedbacks();
                e.explode();
                f.doExplosion(transform.position);
                if (cracks != null) cracks.SetActive(false);
            }
        }

        public void OnHit(Vector2 hitDir, PlayerStage playerStage)
        {
            hitFeedbacks?.PlayFeedbacks();
            print($"hit! object on top :{isObjectOnTop}");
            CoreManager.Instance.AudioManager.PlayOneShot(hitSound, transform.position);
            if (isObjectOnTop && objectOnTop != null)
            {

                Debug.Log($"Adding force: {accumulatedForce}");
                objectOnTop.AddForce(dir.normalized * accumulatedForce);
                accumulatedForce += addedForce;
            }

            if (playerStage == PlayerStage.FinalForm && ++currentHits == hitsToDestroy)
            {
                print("call break on pillar");
                OnBreak();
            }
        }

        public void ResetToInitialState()
        {
            print("reset hill!");
            gameObject.SetActive(true);
            transform.position = startingPos;
            accumulatedForce = baseForce;
            currentHits = 0;
            if (cracks != null) cracks.SetActive(true);
        }
    }
}
