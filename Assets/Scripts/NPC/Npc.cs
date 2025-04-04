using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Npc : MonoBehaviour
    {

        [SerializeReference,SubclassSelector] private List<NpcAction> actionQueue;
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
            if (actionIndex < actionQueue.Count)
            {
                currentAction = actionQueue[actionIndex++];
                currentAction.StartAction(this);
            }
            else
            {
                currentAction = null;
            }
        }

        // ✅ Insert actions dynamically
        public void AddAction(NpcAction newAction, int index = -1)
        {
            if (index == -1 || index >= actionQueue.Count)
                actionQueue.Add(newAction); // Default: Add at the end
            else
                actionQueue.Insert(index, newAction); // Insert at specific position
        }

        // ✅ Interrupt current action and insert a new one
        public void InterruptWithAction(NpcAction newAction)
        {
            currentAction?.ResetAction();
            actionQueue.Insert(actionIndex, newAction); // Insert at current position
            NextAction();
        }
    }
}