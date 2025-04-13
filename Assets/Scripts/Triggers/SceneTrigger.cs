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

        public override void OnTriggerEnter2D(Collider2D other) 
        {
            print($"{other.tag} hit {gameObject.name}");
            if (other.CompareTag(trigger) && ++triggered == requiredTriggers)
            {
                print($"{trigger} triggered {gameObject.name}");
                isTriggered = true;
            }
        }
    }
    
}