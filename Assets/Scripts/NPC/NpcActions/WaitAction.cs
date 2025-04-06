using System;
using Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NPC.NpcActions
{
    [Serializable]
    public class WaitAction : NpcAction
    {
        [SerializeField] private Trigger trigger;                 // Trigger to wait for (can still be used)
        [SerializeField] private UnityEvent startCallback;         // Callback to invoke at the start
        [SerializeField] private bool waitForDuration = false;     // Option to wait for duration
        [SerializeField] private float duration = 3f;              // Duration to wait before completing action

        private float timer = 0f;

        public override void StartAction(Npc npc)
        {
            startCallback?.Invoke();
            if (waitForDuration)
            {
                // Reset the timer if waiting for a duration
                timer = duration;
            }
        }

        public override void UpdateAction(Npc npc)
        {
            if (waitForDuration)
            {
                // If waiting for duration, decrease the timer and check if time is up
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    isCompleted = true;
                }
            }
             if (trigger && trigger.IsTriggered)
            {
                // If waiting for the trigger, complete the action when triggered
                isCompleted = true;
            }
        }
    }
}