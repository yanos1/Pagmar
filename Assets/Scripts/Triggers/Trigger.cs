using System;
using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Triggers
{
    public abstract class Trigger : MonoBehaviour, IResettable
    {
        protected bool isTriggered;
        [SerializeField] protected String trigger;
        [SerializeField] protected String trigger1;

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(trigger) || other.CompareTag(trigger1))
            {
                isTriggered = true;
            }
        }

        public bool IsTriggered => isTriggered;

        public virtual void ResetToInitialState()
        {
            isTriggered = false;
        }
    }
}