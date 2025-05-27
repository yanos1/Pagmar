using Interfaces;
using Player;
using UnityEngine;
using System.Collections.Generic;

namespace Terrain.Environment
{
    public class WoodPlankHingedPlatform : MonoBehaviour, IBreakable, IResettable
    {
        [Header("Setup")]
        [SerializeField] private HingeJoint2D hinge;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject woodPlankPrefab;
        [SerializeField] private bool spawnNewPlank = true;

        [Header("Angle Settings")]
        [SerializeField] private float angleThreshold = 87.2f;

        [SerializeField] private LayerMask groundLayer;

        private float initialRotation;
        private bool isFalling = false;
        private Vector3 spawnPoint;
        private Quaternion spawnRotation;
        private int originalLayer;

        private List<GameObject> spawnedPlanks = new List<GameObject>();

        void Start()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (hinge == null) hinge = GetComponent<HingeJoint2D>();

            hinge.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;

            initialRotation = transform.eulerAngles.z;
            spawnPoint = transform.position;
            spawnRotation = transform.rotation;

            originalLayer = gameObject.layer;
        }

        void Update()
        {
            if (!isFalling) return;

            float currentAngle = Mathf.Abs(Mathf.DeltaAngle(initialRotation, transform.eulerAngles.z));

            if (currentAngle >= angleThreshold)
            {
                StopMovement();
                gameObject.layer = 3; //ground
                isFalling = false;

                if (spawnNewPlank)
                    SpawnNewPlank();
            }
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            if (isFalling) return;

            isFalling = true;
            hinge.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void OnBreak()
        {
            // Optional: handle break behavior
        }

        private void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            hinge.enabled = false;

            float z = transform.eulerAngles.z;
            float normalizedZ = (z + 360f) % 360f;

            if (!Mathf.Approximately(normalizedZ, 180f))
            {
                Vector3 fixedEuler = transform.eulerAngles;
                fixedEuler.z = 180f;
                transform.eulerAngles = fixedEuler;
            }
        }

        private void SpawnNewPlank()
        {
            if (woodPlankPrefab != null)
            {
                GameObject newPlank = Instantiate(woodPlankPrefab, spawnPoint, spawnRotation);
                spawnedPlanks.Add(newPlank);
            }
            else
            {
                Debug.LogWarning("WoodPlankPrefab not assigned!");
            }
        }

        public void ResetToInitialState()
        {
            // Destroy any spawned planks
            foreach (var plank in spawnedPlanks)
            {
                if (plank != null)
                {
                    Destroy(plank);
                }
            }
            spawnedPlanks.Clear();

            // Reset transform
            transform.position = spawnPoint;
            transform.rotation = spawnRotation;

            // Reset Rigidbody and Hinge states
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            hinge.enabled = false;

            // Reset layer
            gameObject.layer = originalLayer;

            // Reset flags
            isFalling = false;
        }
    }
}
