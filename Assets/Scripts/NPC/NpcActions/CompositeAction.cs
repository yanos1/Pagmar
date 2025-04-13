namespace NPC.NpcActions
{
    using System.Collections.Generic;
    using UnityEngine;

    namespace NPC.NpcActions
    {
        [System.Serializable]
        public class CompositeAction : NpcAction
        {
            [SerializeReference, SubclassSelector] 
            private List<NpcAction> subActions = new List<NpcAction>();

            private int currentSubActionIndex = 0;
            private NpcAction currentSubAction;

            public override void StartAction(Npc npc)
            {
                currentSubActionIndex = 0;
                if (subActions.Count > 0)
                {
                    currentSubAction = subActions[currentSubActionIndex];
                    currentSubAction.StartAction(npc);
                }
            }

            public override void UpdateAction(Npc npc)
            {
                if (currentSubAction == null) return;

                currentSubAction.UpdateAction(npc);
                if (currentSubAction.IsCompleted)
                {
                    currentSubAction.ResetAction(npc);
                    currentSubActionIndex++;
                    if (currentSubActionIndex < subActions.Count)
                    {
                        currentSubAction = subActions[currentSubActionIndex];
                        currentSubAction.StartAction(npc);
                    }
                    else
                    {
                        currentSubAction = null;
                        isCompleted = true;
                    }
                }
            }

            public override void ResetAction(Npc npc)
            {
                currentSubAction = null;
                currentSubActionIndex = 0;
            }

            public void AddSubAction(NpcAction action)
            {
                subActions.Add(action);
            }
        }
    }

}