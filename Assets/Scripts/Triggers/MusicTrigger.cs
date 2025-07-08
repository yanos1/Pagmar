using System;
using Interfaces;
using Managers;
using SpongeScene;
using UnityEngine;


namespace Triggers
{
    public class MusicTrigger : MonoBehaviour
    {
        [SerializeField] private MusicType type;
        [SerializeField] private String activatorTag;
        [SerializeField] private float delayBeforeStartMusic;
        [SerializeField] private bool stopMusic;
        private bool triggered = false;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggered && other.CompareTag(activatorTag))
            {
                if (stopMusic)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.StopMusic, null);
                    triggered = true;
                    return;
                }
                // else we are going to switch music
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(delayBeforeStartMusic,
                    () => CoreManager.Instance.EventManager.InvokeEvent(EventNames.ChangeMusic, type)));
                triggered = true;
            }
        }
    }
}