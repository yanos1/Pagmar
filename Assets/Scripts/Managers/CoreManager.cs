
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
        public MonoRunner Runner;
        public PlayerManager Player;

        public CoreManager()
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
            ResetManager = new ResetManager();
            Runner = new GameObject("CoreManagerRunner").AddComponent<MonoRunner>();
        }
    }
}