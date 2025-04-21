using Managers;
using NPC;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Triggers
{
    public class Checkpoint : MonoBehaviour
    {
        private Vector3 recordedPlayerVelocity;
        private Vector3 recordedPlayerPposition;
        
        private Npc recordedNpc;
        private int recorededNpcCurrentActionIndex;
        private Vector3 recordedNpcPposition;
        
        
        public void RestoreCheckpointState()
        {
            if (recordedNpc is not null)
            {
                print($"12 npc location after reset {recordedNpcPposition}");
                recordedNpc.transform.position = recordedNpcPposition;
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

            }

            CoreManager.Instance.ResetManager.UpdateCheckPoint(this);
        }
        
        
    }
}