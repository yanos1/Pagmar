using System;

namespace NPC.NpcActions
{
    [Serializable]
    public abstract class NpcAction 
    {
        protected bool isCompleted = false;

        public abstract void StartAction(Npc npc);
        public abstract void UpdateAction(Npc npc);
        
        public virtual void ResetAction(Npc npc) => isCompleted = false;
        public bool IsCompleted() => isCompleted;
    }

}