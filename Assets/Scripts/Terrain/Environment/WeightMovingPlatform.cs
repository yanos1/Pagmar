﻿using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Managers;
using Player;

namespace Terrain.Environment
{
    public class WeightMovingPlatform : MovingPlatform
    {
        [SerializeField] private List<Transform> wheels; // Assign in inspector
        [SerializeField] private float wheelRotationSpeed = 360f; // degrees per second
        private float directionSign = 1f;

        public override void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.GetComponent<PlayerMovement>() is { } playerManager)
            {
                if (!isMoving)
                {
                    StartCoroutine(DelayedMove());
                }
                
                print($"p{playerManager.GroundCheckPos.y +0.2f} e {col.bounds.max.y}");
                if (playerManager.transform.position.y +0.2f> col.bounds.max.y)
                {
                    print("setting parent elevator");
                    c.collider.transform.SetParent(transform);
                }

            }
        }

        private IEnumerator DelayedMove()
        {
            yield return new WaitForSeconds(1f);

            // Determine direction sign based on triggerDirection.x


            directionSign = Mathf.Sign(triggerDirection.x);
            if (directionSign == 0f) directionSign = 1f; // default fallback
            
            MovePlatformExternally(true);
            
        }

        public override void Update()
        {
            base.Update();
            if (isMoving)
            {
                RotateWheels();
            } else if (isReturning)
            {
                RotateWheels(true);
            }
            
        }

        private void RotateWheels(bool backwards = false)
        {
            float rotationAmount = -wheelRotationSpeed * directionSign * Time.deltaTime;

            foreach (var wheel in wheels)
            {
                wheel.Rotate(backwards ? Vector3.back : Vector3.forward, rotationAmount);
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            directionSign = 1f;

            foreach (var wheel in wheels)
            {
                wheel.localRotation = Quaternion.identity;
            }
        }
    }
}