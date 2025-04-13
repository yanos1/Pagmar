using System.Collections;
using UnityEngine;

using UnityEngine.Serialization;

namespace Camera
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;
        public GameObject Player;
        [SerializeField] private UnityEngine.Camera mainCamera;

        public float SmoothSpeed = 3;
        public static float PPUScale;
        public static float ScreenPPU;
        public Vector3 DefaultCameraPosition;
        public int NativePPU;

        private float _zStartPositionCamera;
        [SerializeField] private float _xOffsetCameraToPlayer;
        [SerializeField] private float _yOffsetCameraToPlayer;
        [SerializeField] private float _zOffsetCameraToPlayer;
        private float _orthographicCameraSize;
        private int _screenResolutionWidth;
        private int _nativeResolutionWidth;
        
        private Coroutine shakeCoroutine;

        private Coroutine cameraLerpCoroutine; // Track the active coroutine

        void Start()
        {
            Instance = this;
            ScreenPPU = NativePPU;
            _zStartPositionCamera = mainCamera.transform.position.z;
            DefaultCameraPosition = new Vector3(2.5f, 2.42f, 0.2f);

        }
        void Update()
        {
            if (_screenResolutionWidth != Screen.currentResolution.width ||
                !Mathf.Approximately(_orthographicCameraSize, mainCamera.orthographicSize))
            {
                UpdatePixelPerfectScaleValues();
            }
        }

        void FixedUpdate()
        {
            if (Mathf.Abs(Player.transform.position.x - GrafikAndGuiSettings.PPV(transform.position.x)) >
                10 * 1 / GrafikAndGuiSettings.ScreenPPU)
            {
                SmoothPixelPerfectCamera();
            }
        }

        private void SmoothPixelPerfectCamera()
        {
            Vector2 desiredPosition = new Vector2(Player.transform.position.x + _xOffsetCameraToPlayer,
                Player.transform.position.y + _yOffsetCameraToPlayer);
            Vector2 pixelPerfectDesiredPosition = new Vector2(PPV(desiredPosition.x), PPV(desiredPosition.y));
            Vector2 smoothPosition = Vector2.Lerp(transform.position, pixelPerfectDesiredPosition,
                Time.deltaTime * SmoothSpeed);
            Vector2 smoothPixelPerfectPosition = new Vector2(PPV(smoothPosition.x), PPV(smoothPosition.y));

            mainCamera.transform.position = (Vector3)smoothPixelPerfectPosition +
                                            Vector3.forward * (PPV(_zStartPositionCamera) * _zOffsetCameraToPlayer);
        }

        public static float PPV(float valueWithoutPixelPerfection)
        {
            float screenPixelPosition = valueWithoutPixelPerfection * ScreenPPU;
            return Mathf.Round(screenPixelPosition) / ScreenPPU;
        }

        private void UpdatePixelPerfectScaleValues()
        {
            float aspectRatio = 16f / 9f;
            float auxiliaryVar = aspectRatio * mainCamera.orthographicSize * NativePPU * 2f;
            _nativeResolutionWidth = (int)auxiliaryVar;

            PPUScale = (float)Screen.currentResolution.width / _nativeResolutionWidth;
            ScreenPPU = PPUScale * NativePPU;

            _orthographicCameraSize = mainCamera.orthographicSize;
            _screenResolutionWidth = Screen.currentResolution.width;
        }

        public void LerpCameraPosition(Vector3 target)
        {
            if (cameraLerpCoroutine != null)
            {
                StopCoroutine(cameraLerpCoroutine);
            }
            cameraLerpCoroutine = StartCoroutine(LerpCameraToTarget(target));
        }

        private IEnumerator LerpCameraToTarget(Vector3 target)
        {
            float elapsedTime = 0f;
            Vector3 startPosition = new Vector3(_xOffsetCameraToPlayer, _yOffsetCameraToPlayer, _zOffsetCameraToPlayer);

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime;
                Vector3 newPosition = Vector3.Lerp(startPosition, target, elapsedTime / 0.7f);
                
                _xOffsetCameraToPlayer = newPosition.x;
                _yOffsetCameraToPlayer = newPosition.y;
                _zOffsetCameraToPlayer = newPosition.z;

                yield return null;
            }

            _xOffsetCameraToPlayer = target.x;
            _yOffsetCameraToPlayer = target.y;
            _zOffsetCameraToPlayer = target.z;
        }
        
        
        // Screen Shake Functionality
        public void ShakeCamera(float duration, float magnitude)
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }
            shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

        public void ShakeCamera()
        {
            ShakeCamera(0.1f,0.1f); 
        }
        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            float elapsedTime = 0f;
            Vector3 originalPosition = mainCamera.transform.localPosition;

            while (elapsedTime < duration)
            {
                float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                mainCamera.transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mainCamera.transform.localPosition = originalPosition; // Reset camera position
        }
    }

    public static class GrafikAndGuiSettings
    {
        public static float ScreenPPU = 100f;

        public static float PPV(float value)
        {
            return value * ScreenPPU;
        }
    }
}
