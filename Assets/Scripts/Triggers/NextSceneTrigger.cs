using Managers;
using Player;
using UnityEngine;

namespace Triggers
{
    public class NextSceneTrigger : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerMovement>() is not null)
            {
                // int nextScene = CoreManager.Instance.SceneManager.LoadNextScene();
            }
        }
    }
}
