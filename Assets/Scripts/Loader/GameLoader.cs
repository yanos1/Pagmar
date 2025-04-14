using Managers;
using UnityEngine;

namespace Loader
{
    public class GameLoader : MonoBehaviour
    {
        // will be changed
        [SerializeField] private ResetManager resetManager;
        void Awake()
        {
            new CoreManager(resetManager);
        }
    }
}
