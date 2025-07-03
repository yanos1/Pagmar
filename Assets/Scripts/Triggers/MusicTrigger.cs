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
        private bool triggered = false;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (!triggered && other.CompareTag(activatorTag))
            {
                StartCoroutine(UtilityFunctions.WaitAndInvokeAction(delayBeforeStartMusic,
                    () => CoreManager.Instance.EventManager.InvokeEvent(EventNames.ChangeMusic, type)));
                triggered = true;
            }
        }
    }
}