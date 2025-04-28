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
        private void Awake()
        {
            new CoreManager(resetManager, uiManager, poolManager,cameraManager);
        }
    }
}