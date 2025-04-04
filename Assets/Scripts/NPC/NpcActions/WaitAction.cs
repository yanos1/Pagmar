
using System;
using Triggers;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class WaitAction : NpcAction
    {
        [SerializeField] private Trigger trigger;
        public override void StartAction(Npc npc)
        {
            // add boredom 
        }

        public override void UpdateAction(Npc npc)
        {
            if (trigger.IsTriggered)
            {
                isCompleted = true;
            }
        }
    }
    
}