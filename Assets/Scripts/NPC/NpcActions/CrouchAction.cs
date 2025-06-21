using System;
using NUnit.Framework.Constraints;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class CrouchAction : NpcAction
    {
        [SerializeField] private Collider2D crouchCol;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            crouchCol.enabled = true;
            npc.SetState(NpcState.Crouching);
            isCompleted = true;
        }

        public override void UpdateAction(Npc npc)
        {
        }

        public override void ResetAction(Npc npc)
        {
            isCompleted = false;
            afterActionCallback?.Invoke();
            crouchCol.enabled = false;
        }
    }
}