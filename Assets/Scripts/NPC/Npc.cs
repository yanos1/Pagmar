using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enemies;
using Interfaces;
using NPC.NpcActions;
using Terrain;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
    public class Npc : MonoBehaviour
    {
        [SerializeReference,SubclassSelector] private List<NpcAction> actions;

        private NpcState state;
        private NpcAction currentAction;
        private int actionIndex = 0;
        private Coroutine currentCoroutine;
        private bool isGrounded;
        private float groundCheckDistance = 0.1f;
        private Rigidbody2D rb;

        [SerializeField] private float speed;
        [SerializeField] private float maxJumpHeight;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float dashDistance;

        public float MaxJumpHeight => maxJumpHeight;
        public float JumpDuration => jumpDuration;
        public float DashDistance => dashDistance;
        public float Speed => speed;
        public NpcState State => state;
        public Rigidbody2D Rb => rb;
        public int ActionIndex => actionIndex;
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            NextAction();
        }

        private void Update()
        {
            if (currentAction != null)
            {
                currentAction.UpdateAction(this);
                // print(currentAction);
                if (currentAction.IsCompleted && currentCoroutine == null)
                {
                    print($"{currentAction.ToString()} is complete!");
                    currentCoroutine = StartCoroutine(WaitAndMoveToNextAction(currentAction.DelayAfterAction));
                }
            }

            if (state != NpcState.Jumping && state != NpcState.Idle)
            {
                isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance,
                    LayerMask.NameToLayer("Ground"));
                Debug.DrawRay(transform.position, Vector2.down * groundCheckDistance, isGrounded? Color.green : Color.red);


                if (!isGrounded)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
        
        private IEnumerator WaitAndMoveToNextAction(float delayAfterAction)
        {
            yield return new WaitForSeconds(delayAfterAction);  // Wait for 1.2 seconds
            NextAction();  // Move to the next action
            currentCoroutine = null;  // Reset the coroutine to allow further actions
        }

        private void NextAction()
        {
            if (actionIndex < actions.Count)
            {
                print($"{actionIndex} {actions.Count}");
                if (actionIndex > 0)
                {
                    currentAction.ResetAction(this);
                }
                currentAction = actions[actionIndex++];
                print(currentAction);
                currentAction.StartAction(this);
            }
            else
            {
                currentAction = null;
            }
        }
        
        public void RestoreStateFromIndex(int index)
        {
            if (index >= 0 && index < actions.Count)
            {
                // Reset the current action if needed
                currentAction?.ResetAction(this);

                // Set index and start the correct action
                actionIndex = index;
                currentAction = actions[actionIndex];
                currentAction.StartAction(this);
                print($"12 starting action {currentAction} after reset");
            }
        }
        
        public void AddAction(NpcAction newAction)
        {
            actions.Insert(actionIndex, newAction);
        }

        public void InterruptWithAction(NpcAction newAction)
        {
            actionIndex--;
            currentAction?.ResetAction(this);
            actions.Insert(actionIndex, newAction); // Insert at current position
            NextAction();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            IBreakable breakable = other.GetComponent<IBreakable>();
            print($"triggered hit on {other.gameObject.name}");
            if (breakable is not null)
            {
                print("triggered hit on breakkable!");
                print($"npc state is {state}");
            }
            if (breakable is not null && state == NpcState.Charging)
            {
                print("triggeredBreakable");
                breakable.OnBreak();
                return;
            }

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy is not null)
            {
                print("RAM ENEMY");
                enemy.OnRam();
            }
        }

        public void SetState(NpcState newState)
        {
            state = newState;
        }
        
    }


    public enum NpcState
    {
        Idle, Walking, Jumping, Charging, Following, Followed
    }
}