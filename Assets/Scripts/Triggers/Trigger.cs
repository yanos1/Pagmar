using System;
using Interfaces;
using SpongeScene;
using UnityEngine;

namespace Triggers
{
    public abstract class Trigger : MonoBehaviour, IResettable
    {
        protected bool isTriggered;
        [SerializeField] protected String trigger;
        private Collider2D col;

        private void Start()
        {
            col = GetComponent<Collider2D>();
        }

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
            col.enabled = false;
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.1f, () => col.enabled = true));
        }
    }
}