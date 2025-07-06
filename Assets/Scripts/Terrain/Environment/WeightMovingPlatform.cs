using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Player;

namespace Terrain.Environment
{
    public class WeightMovingPlatform : MovingPlatform
    {
        [SerializeField] private List<Transform> wheels; // Assign in inspector
        [SerializeField] private float wheelRotationSpeed = 360f; // degrees per second
        private bool isMoving;
        private float directionSign = 1f;

        private void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.GetComponent<PlayerManager>() is { } playerManager)
            {
                if (!hasMoved)
                {
                    StartCoroutine(DelayedMove());
                }
            }
        }

        private IEnumerator DelayedMove()
        {
            yield return new WaitForSeconds(1f);

            // Determine direction sign based on triggerDirection.x


            directionSign = Mathf.Sign(triggerDirection.x);
            if (directionSign == 0f) directionSign = 1f; // default fallback


            isMoving = true;
            MovePlatformExternally();
        }

        private void Update()
        {
            if (isMoving)
            {
                RotateWheels();
            }
        }

        private void RotateWheels()
        {
            float rotationAmount = -wheelRotationSpeed * directionSign * Time.deltaTime;

            foreach (var wheel in wheels)
            {
                wheel.Rotate(Vector3.forward, rotationAmount);
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            isMoving = false;
            directionSign = 1f;

            foreach (var wheel in wheels)
            {
                wheel.localRotation = Quaternion.identity;
            }
        }
    }
}