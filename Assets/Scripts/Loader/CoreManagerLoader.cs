using System;
using Camera;
using Managers;
using UnityEngine;

namespace Loader
{
    public class CoreManagerLoader : MonoBehaviour
    {
        [SerializeField] private ResetManager resetManager;
        [SerializeField] private UiManager uiManager;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private GameManager _gameManager;
        private void Awake()
        {
            new CoreManager(resetManager, uiManager, poolManager,cameraManager, audioManager, _gameManager);
        }
    }
}