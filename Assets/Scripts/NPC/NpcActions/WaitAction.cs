using System;
using System.Collections;
using Managers;
using SpongeScene;
using Triggers;
// using Unity.Android.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NPC.NpcActions
{
    [Serializable]
    public class WaitAction : NpcAction
    {
        [SerializeField] private Trigger trigger;                 // Trigger to wait for (can still be used)
        [SerializeField] private bool waitForDuration = false;     // Option to wait for duration
        [SerializeField] private float duration = 3f;// Duration to wait before completing action
        private Coroutine c;

        private float timer = 0f;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            // npc.SetState(NpcState.Idle);
            if (waitForDuration)
            {
                // Reset the timer if waiting for a duration
                timer = duration;
            }
            CoreManager.Instance.Runner.StopAndStartCoroutine(ref c , TurnAroundToPlayerIfNeeded(npc));

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
             if (trigger && trigger.IsTriggered && isCompleted == false)
            {
                // If waiting for the trigger, complete the action when triggered
                isCompleted = true;

            }

        }

        private IEnumerator TurnAroundToPlayerIfNeeded(Npc npc)
        {
            while (isCompleted == false)
            {
                npc.TurnAround(CoreManager.Instance.Player.transform.position - npc.transform.position);
                yield return new WaitForSeconds(1.3f);
            }

        }

        public override void ResetAction(Npc npc)
        {
            CoreManager.Instance.Runner.StopCoroutine(c);

            base.ResetAction(npc);
        }
    }
}