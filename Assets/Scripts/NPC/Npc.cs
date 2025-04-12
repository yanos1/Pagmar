using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        private Coroutine currentCoroutine;

        [SerializeField] private float speed;
        [SerializeField] private float maxJumpHeight;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float dashDistance;

        public float MaxJumpHeight => maxJumpHeight;
        public float JumpDuration => jumpDuration;
        public float DashDistance => dashDistance;
        public float Speed => speed;


        private void Start()
        {
            NextAction();
        }

        private void Update()
        {
            if (currentAction != null)
            {
                currentAction.UpdateAction(this);
                print(currentAction);
                if (currentAction.IsCompleted() && currentCoroutine == null)
                {
                    print($"{currentAction.ToString()} is complete!");
                    currentCoroutine = StartCoroutine(WaitAndMoveToNextAction());
                }
            }
        }
        
        private IEnumerator WaitAndMoveToNextAction()
        {
            yield return new WaitForSeconds(0.2f);  // Wait for 1.2 seconds
            NextAction();  // Move to the next action
            currentCoroutine = null;  // Reset the coroutine to allow further actions
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
            actionIndex--;
            currentAction?.ResetAction(this);
            actions.Insert(actionIndex, newAction); // Insert at current position
            NextAction();
        }
        
        
        
    }
}