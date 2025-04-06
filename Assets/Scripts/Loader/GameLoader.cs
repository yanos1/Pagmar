using Managers;
using UnityEngine;

namespace Loader
{
    public class GameLoader : MonoBehaviour
    {
        // will be changed
        void Awake()
        {
            new CoreManager();
        }
    }
}
