using Managers;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Triggers
{
    public class SceneTrigger : Trigger
    {
        [Header("Trigger Settings")] private int triggered = 0;
        [SerializeField] private int requiredTriggers;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerManager>())
            {
                if (++triggered == requiredTriggers)
                {
                    print($"trigged {gameObject.name} trigger");
                    isTriggered = true;
                }
            }
        }
    }
}