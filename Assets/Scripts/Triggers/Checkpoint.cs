using JetBrains.Annotations;
using Managers;
using MoreMountains.Feedbacks;
using NPC;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Triggers
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private PlayerManager.PlayerStage newStage;
        private Vector3 recordedPlayerVelocity;
        private Vector3 recordedPlayerPposition;
        
        private Npc recordedNpc;
        private int recorededNpcCurrentActionIndex;
        private Vector3 recordedNpcPposition;
        
        [SerializeField] MMF_Player checkpointFeedback;

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
            if (player is not null)
            {
                
                recordedNpc = player.GetFollowedBy();
                if (recordedNpc is not null)
                {
                    print("recorded npc");
                    recorededNpcCurrentActionIndex = recordedNpc.ActionIndex;
                    recordedNpcPposition = recordedNpc.transform.position;
                }
                print("recorded player");
                recordedPlayerPposition = player.transform.position;
                print("play feebacks");
                checkpointFeedback?.PlayFeedbacks();
                CoreManager.Instance.ResetManager.UpdateCheckPoint(this);
                if (newStage != PlayerManager.PlayerStage.None)
                {
                    player.SetPlayerStage(newStage);
                }

            }

        }
        
        
    }
}