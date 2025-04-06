
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
        public CallbackManager CallbackManager;
        public MonoRunner Runner;
        public PlayerManager Player;

        public CoreManager()
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
            Runner = new GameObject("CoreManagerRunner").AddComponent<MonoRunner>();
        }
    }
}