using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
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
        [SerializeField] private EventReference breakSound;

        [Tooltip("World position where the new hinge will be created")]
        [SerializeField] private Transform spawnPoint;

        private float initialRotation;
        private bool isFalling = false;
        private const float angleThreshold = 90f;

        private EventInstance breakSoundInstance;

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
            // Optional: handle destruction
        }

        public void OnHit(Vector2 hitDir, PlayerStage stage)
        {
            if (isFalling) return;

            rb.bodyType = RigidbodyType2D.Dynamic;
            hinge.enabled = true;
            _woodPlank.ActivateHinge();
            isFalling = true;

            breakSoundInstance = CoreManager.Instance.AudioManager.CreateEventInstance(breakSound);
            breakSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            breakSoundInstance.start();
        }

        private void StopMovement()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            hinge.enabled = false;

            breakSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            breakSoundInstance.release();
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
