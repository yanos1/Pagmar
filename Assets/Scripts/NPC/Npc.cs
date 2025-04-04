using System.Collections.Generic;
using NPC.NpcActions;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class Npc : MonoBehaviour
    {

        [SerializeReference,SubclassSelector] private List<NpcAction> actions;
        private NpcAction currentAction;
        private int actionIndex = 0;

        private void Start()
        {
            NextAction();
        }

        private void Update()
        {
            if (currentAction != null)
            {
                currentAction.UpdateAction(this);
                if (currentAction.IsCompleted())
                    NextAction();
            }
        }

        private void NextAction()
        {
            if (actionIndex < actions.Count)
            {
                currentAction = actions[actionIndex++];
                currentAction.StartAction(this);
            }
            else
            {
                currentAction = null;
            }
        }

        // ✅ Insert actions dynamically
        public void AddAction(NpcAction newAction)
        {
            actions.Insert(actionIndex, newAction);
        }

        // ✅ Interrupt current action and insert a new one
        public void InterruptWithAction(NpcAction newAction)
        {
            currentAction?.ResetAction(this);
            actions.Insert(actionIndex, newAction); // Insert at current position
            NextAction();
        }
    }
}