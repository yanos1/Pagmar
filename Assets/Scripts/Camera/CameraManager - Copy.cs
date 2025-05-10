using System;
using System.Collections;
using Managers;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Camera
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera[] _allVirtualCameras;

        [Header("Controls for lerping the Y Damping during player jump/fall")] [SerializeField]
        private float _fallPanAmount = 0.25f;

        [SerializeField] private float _fallYPanTime = 0.35f;
        [SerializeField] public float _fallSpeedYDampingChangeThreshold = -5f;

        public bool IsLerpingYDamping { get; private set; }
        public bool LerpedFromPlayerFalling { get; set; }

        private Coroutine _lerpYPanCoroutine;
        private Coroutine _panCameraCoroutine;

        private CinemachineCamera _currentCamera;
        private CinemachinePositionComposer _framingTransposer;

        private static CameraManager instance;

        private float _normYPanAmount;
        private Vector2 _startingTrackedObjectOffset;

        public static CameraManager GetInstance()
        {
            if (instance == null)
            {
                Debug.LogError("CameraManager instance is null.");
                return null;
            }

            return instance;
        }

        public float FallSpeedYDampingChangeThreshold
        {
            get => _fallSpeedYDampingChangeThreshold;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

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
                    _startingTrackedObjectOffset = _framingTransposer.TargetOffset;
                }
            }



        }

        public void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, SetCamerasOfTheScene);
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

        public void SetCamerasOfTheScene(object obj)
        {
            _allVirtualCameras = FindObjectsOfType<CinemachineCamera>();
            
            foreach (var vc in _allVirtualCameras)
            {
                if (vc.isActiveAndEnabled)
                {
                    _currentCamera = vc;
                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                    _normYPanAmount = _framingTransposer.Damping.y;
                    _startingTrackedObjectOffset = _framingTransposer.TargetOffset;
                    break;
                }
            }
        }

        public void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, SetCamerasOfTheScene);
        }

        #endregion

        public void PanCameraOnContact(float panDisntance, float panTime, PanDirection panDirection,
            bool panToStartingPos)
        {
            _panCameraCoroutine = StartCoroutine(PanCamera(panDisntance, panTime, panDirection, panToStartingPos));
        }

        private IEnumerator PanCamera(float panDisntance, float panTime, PanDirection panDirection,
            bool panToStartingPos)
        {

            Debug.Log(panToStartingPos);
            Vector2 endPos = Vector2.zero;
            Vector2 startPos = Vector2.zero;

            if (!panToStartingPos)
            {
                switch (panDirection)
                {
                    case PanDirection.Up:
                        endPos = Vector2.up;
                        break;
                    case PanDirection.Down:
                        endPos = Vector2.down;
                        break;
                    case PanDirection.Left:
                        endPos = Vector2.left;
                        break;
                    case PanDirection.Right:
                        endPos = Vector2.right;
                        break;
                    default:
                        break;
                }

                endPos *= panDisntance;
                startPos = _startingTrackedObjectOffset;
                endPos += startPos;
            }
            else
            {
                startPos = _framingTransposer.TargetOffset;
                endPos = _startingTrackedObjectOffset;
            }

            float elapsedTime = 0f;
            while (elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                Vector2 lerpedPos = Vector2.Lerp(startPos, endPos, (elapsedTime / panTime));
                _framingTransposer.TargetOffset = lerpedPos;

                yield return null;
            }

        }

        public void SwapCamera(CinemachineCamera cameraFromEnter, CinemachineCamera cameraFromExit,
            Vector2 triggerExitDirection, bool isVerticalSwap)
        {
            if (!isVerticalSwap)
            {
                if (_currentCamera == cameraFromEnter && triggerExitDirection.x > 0f)
                {
                    cameraFromExit.enabled = true;

                    cameraFromEnter.enabled = false;

                    _currentCamera = cameraFromExit;

                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                }
                else if (_currentCamera == cameraFromExit && triggerExitDirection.x < 0f)
                {
                    cameraFromEnter.enabled = true;

                    cameraFromExit.enabled = false;

                    _currentCamera = cameraFromEnter;

                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                }
            }
            else
            {
                if (_currentCamera == cameraFromEnter && triggerExitDirection.y < 0f)
                {
                    cameraFromExit.enabled = true;

                    cameraFromEnter.enabled = false;

                    _currentCamera = cameraFromExit;

                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                }
                else if (_currentCamera == cameraFromExit && triggerExitDirection.y > 0f)
                {
                    cameraFromEnter.enabled = true;

                    cameraFromExit.enabled = false;

                    _currentCamera = cameraFromEnter;

                    _framingTransposer = _currentCamera.GetComponentInParent<CinemachinePositionComposer>();
                }
            }
        }
    }
}
