using Interfaces;
using Managers;
using UnityEngine;


namespace Triggers
{
    public class LevelAmbienceTrigger : MonoBehaviour
    {
        [SerializeField] private AmbienceType ambienceType;
        private bool triggered = false;
        public void OnTriggerEnter2D(Collider2D other) 
        {
            if (!triggered && other.CompareTag("Player"))
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.ChangeAmbience, ambienceType);
                triggered = true;
            } 
        }
    }
    
}