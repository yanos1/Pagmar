﻿using NPC.BigFriend;
using UnityEngine;

namespace NPC.NpcActions
{
    using System;


    [Serializable]
    public class SleepAction : NpcAction
    {
        [SerializeField] private BigActions _actions;
        private bool displayingSleep = false;
        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Sleeping);
        }

        public override void UpdateAction(Npc npc)
        {
            if (npc.IsJumpedOn && !displayingSleep)
            {
                displayingSleep = true;
                _actions.ShowSleepHeallRequest();
            }
            else if (npc.IsJumpedOn == false)
            {
                displayingSleep = false;
            }
        }

        public override void ResetAction(Npc npc)
        {
            isCompleted = false;
            afterActionCallback?.Invoke();
        }
    }
}