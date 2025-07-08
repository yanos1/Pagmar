using Managers;
using Player;
using SpongeScene;
using UnityEngine;

namespace Triggers
{
    public class NextSceneTrigger : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private bool triggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>() is not null && !triggered)
            {
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(1f, () =>
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.StartLoadNextScene, null)));
                ScenesManager.Instance.LoadNextScene();
                triggered = true;
            }
        }
    }
}