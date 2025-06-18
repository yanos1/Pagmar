using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    public class ZoomingInAndOurCamera : MonoBehaviour
    {
        private CinemachineCamera cineCam;
        private CinemachinePositionComposer composer;

        // Store original values
        private float originalCameraDistance;
        private Vector3 originalTargetOffset;

        private void Start()
        {
            cineCam = GetComponent<CinemachineCamera>();
            composer = GetComponent<CinemachinePositionComposer>();

            if (composer != null)
            {
                originalCameraDistance = composer.CameraDistance;
                originalTargetOffset = composer.TargetOffset;
            }
        }

        public void ZoomIn()
        {
            StartCoroutine(SmoothZoom(2f, 17f, new Vector3(0, 1.5f, 0)));
        }
        
        public void ZoomOut()
        {
            StartCoroutine(SmoothZoom(2f, 17f, new Vector3(0, 1.5f, 0)));
        }
        

        public void ResetZoom()
        {
            print("12 reset zoom");
            StartCoroutine(SmoothZoom(0.2f, originalCameraDistance, originalTargetOffset));
        }

        private IEnumerator SmoothZoom(float duration, float targetDistance, Vector3 targetOffset)
        {
            if (composer == null) yield break;

            float elapsed = 0f;
            float startDistance = composer.CameraDistance;
            Vector3 startOffset = composer.TargetOffset;
            float startDeadZoneDepth = composer.DeadZoneDepth;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                composer.CameraDistance = Mathf.Lerp(startDistance, targetDistance, t);
                composer.TargetOffset = Vector3.Lerp(startOffset, targetOffset, t);

                yield return null;
            }

            // Ensure final values
            composer.CameraDistance = targetDistance;
            composer.TargetOffset = targetOffset;
        }
    }
}
