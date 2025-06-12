using JetBrains.Annotations;
using Managers;
using MoreMountains.Feedbacks;
using NPC;
using Player;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;

namespace Triggers
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private PlayerStage newStage;
        private Vector3 recordedPlayerVelocity;
        private Vector3 recordedPlayerPposition;
        
        private Npc recordedNpc;
        private int recorededNpcCurrentActionIndex;
        private Vector3 recordedNpcPposition;
        private bool triggered;
        
        [SerializeField] MMF_Player checkpointFeedback;
        [SerializeField] private bool callEventOnReach;
        [SerializeField] private EventNames onReached;
        [SerializeField] private float onReachedDelay;

        public void RestoreCheckpointState()
        {
            if (recordedNpc is not null)
            {
                recordedNpc.transform.position = recordedNpcPposition;
                print($"12 npc location after reset {recordedNpc.transform.position}");

                recordedNpc.RestoreStateFromIndex(recorededNpcCurrentActionIndex);
            }

            if (CoreManager.Instance.Player != null)
            {
                CoreManager.Instance.Player.transform.position = recordedPlayerPposition;
                // You can also reset velocity or other states here if needed
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print("reched checkpoint");
           
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player is not null && !player.IsDead)
            {
                if( triggered ) return;
                triggered = true;
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.ReachedCheckPoint, player.transform.position);

                if (callEventOnReach)
                {
                    StartCoroutine(UtilityFunctions.WaitAndInvokeAction(onReachedDelay,()=> CoreManager.Instance.EventManager.InvokeEvent(onReached, null)));
                }
                
                recordedNpc = player.GetFollowedBy();
                if (recordedNpc is not null)
                {
                    print("recorded npc");
                    recorededNpcCurrentActionIndex = recordedNpc.ActionIndex;
                    recordedNpcPposition = recordedNpc.transform.position;
                }
                print("recorded player");
                recordedPlayerPposition = transform.position;
                print("play feebacks");
                checkpointFeedback?.PlayFeedbacks();
                CoreManager.Instance.ResetManager.UpdateCheckPoint(this);
                if (newStage != PlayerStage.None)
                {
                    player.SetPlayerStage(newStage);
                }

            }

        }
        
        
    }
}