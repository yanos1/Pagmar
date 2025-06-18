using System;
using Interfaces;
using UnityEngine;

namespace Triggers
{
    public abstract class Trigger : MonoBehaviour, IResettable
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

        public virtual void ResetToInitialState()
        {
            isTriggered = false;
        }
    }
}