
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

        public CoreManager(ResetManager resetManager, UiManager uiManager, PoolManager poolManager)
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
            UiManager = uiManager;
            ResetManager = resetManager;
            PoolManager = poolManager;
            Runner = new GameObject("CoreManagerRunner").AddComponent<MonoRunner>();
            
            uiManager.gameObject.SetActive(true); // this acticvates OnEnable which register the manager events to eventmanager after it has finished loading.
            resetManager.gameObject.SetActive(true);
        }
    }
}