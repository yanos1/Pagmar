using System;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public abstract class Trigger : MonoBehaviour
    {
        protected bool isTriggered;
        [SerializeField] protected String trigger;

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(trigger))
            {
                isTriggered = true;
            }
        }

        public bool IsTriggered => isTriggered;
        
    }
}