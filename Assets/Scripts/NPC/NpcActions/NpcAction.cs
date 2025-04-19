using System;
using UnityEngine;
using UnityEngine.Events;

namespace NPC.NpcActions
{
    [Serializable]
    public abstract class NpcAction 
    {        
        
        protected bool isCompleted = false;

        [SerializeField] protected float delayAfterAction;
        [SerializeField] protected UnityEvent beforeActionCallBack;
        [SerializeField] protected UnityEvent afterActionCallback;
        
        
        private const float DefaultDelayAfterAction = 0.2f;

        public bool IsCompleted
        {
            get => isCompleted;
            set => isCompleted = value;
        }
        public float DelayAfterAction
        {
            get
            {
                if (delayAfterAction > 0) return delayAfterAction;
                return DefaultDelayAfterAction;
            }
        }

        public virtual void StartAction(Npc npc)
        {
            IsCompleted = false; // this solves a problem with replicating refrences in the inspector

            beforeActionCallBack?.Invoke();
        }
        public abstract void UpdateAction(Npc npc);

        public virtual void ResetAction(Npc npc)
        {
            Debug.Log($"method called {afterActionCallback?.ToString()}");
            isCompleted = false;
            afterActionCallback?.Invoke();
        }
    }

}