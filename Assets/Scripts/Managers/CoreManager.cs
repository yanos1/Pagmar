
using Camera;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CoreManager
    {
        public static CoreManager Instance;
        
        public EventManager EventManager;
        public InputManager InputManager;
        public ResetManager ResetManager;
        public PoolManager PoolManager;
        public UiManager UiManager;
        public MonoRunner Runner;
        public PlayerManager Player;
        public CameraManager CameraManager;
        public AudioManager AudioManager;
        public PlayerPositionManager PositionManager;
        

        public CoreManager(ResetManager resetManager, UiManager uiManager, PoolManager poolManager,
            CameraManager cameraManager, AudioManager audioManager)
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
            UiManager = uiManager;
            ResetManager = resetManager;
            PoolManager = poolManager;
            AudioManager = audioManager;
            Runner = new GameObject("CoreManagerRunner").AddComponent<MonoRunner>();
            CameraManager = cameraManager;
            
            uiManager.gameObject.SetActive(true); // this acticvates OnEnable which register the manager events to eventmanager after it has finished loading.
            resetManager.gameObject.SetActive(true);
            cameraManager.gameObject.SetActive(true);
            audioManager.gameObject.SetActive(true);
        }
    }
}