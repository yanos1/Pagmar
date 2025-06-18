using Managers;
using UnityEngine;

namespace Triggers
{
    public class ActionTrigger : Trigger
    {
        [SerializeField] private EventNames listenTo;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(listenTo, OnAction);
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(listenTo, OnAction);
        }

        private void OnAction(object obj)
        {
            isTriggered = true;
            Invoke(nameof(ResetToInitialState), 1); // allows us to reuse the same trigger
        }
        
        public override void ResetToInitialState()
        {
            isTriggered = false;
        }
    }
}