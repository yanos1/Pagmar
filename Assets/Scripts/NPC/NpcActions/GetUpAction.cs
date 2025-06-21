using System;
using System.Collections;
using Managers;
using NUnit.Framework.Constraints;
using SpongeScene;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class GetUpAction : NpcAction
    {

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.GetUp);
            CoreManager.Instance.Runner.StartCoroutine(UtilityFunctions.WaitAndInvokeAction(3f, () =>isCompleted = true)); // tiem for animation to end.
        }

        public override void UpdateAction(Npc npc)
        {
        }
    }
}