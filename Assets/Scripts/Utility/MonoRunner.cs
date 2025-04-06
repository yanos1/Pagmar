using UnityEngine;

namespace Managers
{
    public class MonoRunner : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}