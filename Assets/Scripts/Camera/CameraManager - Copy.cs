using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera[] _allVirtualCameras;

        [Header("Controls for lerping the Y Damping during player jump/fall")]
        [SerializeField] private float _fallPanAmount = 0.25f;
        [SerializeField] private float _fallYPanTime = 0.35f;
        [SerializeField] public float _fallSpeedYDampingChangeThreshold = -5f;

        public bool IsLerpingYDamping { get; private set; }
        public bool LerpedFromPlayerFalling { get;  set; }

        private Coroutine _lerpYPanCoroutine;

        private CinemachineCamera _currentCamera;
        private CinemachinePositionComposer _framingTransposer;

        private static CameraManager instance;
        
        private float _normYPanAmount;

        public static CameraManager GetInstance()
        {
            if (instance == null)
            {
                Debug.LogError("CameraManager instance is null.");
                return null;
            }

            return instance;
        }
        
        public float FallSpeedYDampingChangeThreshold {get => _fallSpeedYDampingChangeThreshold; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            for (int i = 0; i < _allVirtualCameras.Length; i++)
            {
                if (_allVirtualCameras[i].enabled)
                {
                    // Set the current active camera
                    _currentCamera = _allVirtualCameras[i];

                    // Set the framing transposer
                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                    _normYPanAmount = _framingTransposer.Damping.y;
                    Debug.Log(_normYPanAmount);
                }
            }
            

        }

        #region Lerp the Y Damping

        public void LerpYDamping(bool isPlayerFalling)
        {
            _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
        }

        private IEnumerator LerpYAction(bool isPlayerFalling)
        {
            IsLerpingYDamping = true;

            // Grab the starting damping amount
            float startDampAmount = _framingTransposer.Damping.y;
            float endDampAmount = 0f;

            // Determine the end damping amount
            if (isPlayerFalling)
            {
                endDampAmount = _fallPanAmount;
                LerpedFromPlayerFalling = true;
            }
            else
            {
                endDampAmount = _normYPanAmount;
            }

            // Lerp the pan amount
            float elapsedTime = 0f;
            while (elapsedTime < _fallYPanTime)
            {
                elapsedTime += Time.deltaTime;

                float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
                
                Vector3 damping = _framingTransposer.Damping;
                damping.y = lerpedPanAmount;
                _framingTransposer.Damping = damping;
                
                yield return null;
            }

            IsLerpingYDamping = false;
        }

        #endregion
    }
}
