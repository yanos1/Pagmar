
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CoreManager
    {

        public static CoreManager Instance;
        
        
        public EventManager EventManager;
        public InputManager InputManager;
        public SceneManager SceneManager;

        private CoreManager()
        {
            Instance ??= this;
            EventManager = new EventManager();
            InputManager = new InputManager();
        }
        
        
    }
}