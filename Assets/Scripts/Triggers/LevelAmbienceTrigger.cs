using Interfaces;
using Managers;
using Player;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Triggers
{
    public class LevelAmbienceTrigger : MonoBehaviour, IResettable
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

        public void ResetToInitialState()
        {
            triggered = false;
        }
    }
    
}