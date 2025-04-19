
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
        public SceneManager SceneManager;
        public ResetManager ResetManager;
        public UiManager UiManager;
        public MonoRunner Runner;
        public PlayerManager Player;

        public CoreManager(ResetManager resetManager, UiManager uiManager)
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
            UiManager = uiManager;
            ResetManager = resetManager;
            Runner = new GameObject("CoreManagerRunner").AddComponent<MonoRunner>();
            uiManager.gameObject.SetActive(true);
            resetManager.gameObject.SetActive(true);
        }
    }
}