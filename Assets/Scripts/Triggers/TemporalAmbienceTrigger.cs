using System;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Triggers
{
    public class TemporalAmbienceTrigger : MonoBehaviour
    {
        [SerializeField] private AmbienceType type;
        [SerializeField] private EventReference ambience;
        [SerializeField] private bool stopAmbience;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (stopAmbience)
                {
                    CoreManager.Instance.AudioManager.RemoveTemporalAmbience(type);
                }
                else
                {
                    CoreManager.Instance.AudioManager.AddTemporalAmbience(type, ambience);
                }
            }
        }
        
        
        public void StopAmbienceExternally()
        {
            CoreManager.Instance.AudioManager.RemoveTemporalAmbience(type);
        }

        public void StartAmbienceExternally()
        {
            CoreManager.Instance.AudioManager.AddTemporalAmbience(type, ambience);
        }
    }
}