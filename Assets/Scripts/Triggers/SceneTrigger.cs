using Managers;
using Player;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Triggers
{
    public class SceneTrigger : Trigger
    {
        [Header("Trigger Settings")] private int triggered = 0;
        [SerializeField] private int requiredTriggers;
        [SerializeField] private EventNames onTrigger;
        

        public override void OnTriggerEnter2D(Collider2D other) 
        {
            print($"triggered by {other.gameObject.name}");
            if (other.CompareTag(trigger) && ++triggered == requiredTriggers)
            {
                print($"{trigger} triggered {gameObject.name}");
                isTriggered = true;
                if (onTrigger != EventNames.None)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onTrigger, null);
                }
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            triggered = 0;
        }
    }
    
}