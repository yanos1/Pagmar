using Interfaces;
using Player;
using UnityEngine;

namespace Terrain.Environment
{
    public class WoodPlank : MonoBehaviour, IBreakable
    {
        [SerializeField] private WoodPlankHinged _woodPlank;
        [SerializeField] private HingeJoint2D hinge;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject woodPlankPrefab;

        [Tooltip("World position where the new hinge will be created")]
        [SerializeField] private Transform spawnPoint;

        private float initialRotation;
        private bool isFalling = false;
        private const float angleThreshold = 90f;

        private void Start()
        {
            initialRotation = transform.eulerAngles.z;
            hinge.enabled = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        private void Update()
        {
            if (!isFalling) return;

            float currentAngle = Mathf.Abs(Mathf.DeltaAngle(initialRotation, transform.eulerAngles.z));

            if (currentAngle >= angleThreshold)
            {
                StopMovement();
                SpawnNewPlank();
                isFalling = false;
            }
        }

        public void OnBreak()
        {
            // Optionally destroy or deactivate the plank
            // gameObject.SetActive(false);
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            if (isFalling) return;

            rb.bodyType = RigidbodyType2D.Dynamic;
            hinge.enabled = true;
            _woodPlank.ActivateHinge();
            isFalling = true;
        }

        private void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            hinge.enabled = false;
        }

        private void SpawnNewPlank()
        {
            if (woodPlankPrefab != null && spawnPoint != null)
            {
                Instantiate(woodPlankPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("Missing woodPlankPrefab or spawnPoint!");
            }
        }
    }
}
