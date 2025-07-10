using System;
using Managers;

namespace NPC.NpcActions
{
    [Serializable]
    public class CrouchAction : NpcAction
    {

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Crouching);
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.BigDoingSomethingNice, null);
            isCompleted = true;
        }

        public override void UpdateAction(Npc npc)
        {
            
        }

        public override void ResetAction(Npc npc)
        {
            isCompleted = false;
            afterActionCallback?.Invoke();
        }
    }
}