using System;
using Player;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public class MountAction : NpcAction
    {
        [SerializeField] private Vector3 mountPosition; // 304.36, 8.01, 1
        [SerializeField] private PlayerManager player;


        public override void UpdateAction(Npc npc)
        {
        }
        
        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            player.transform.position = mountPosition;
            Debug.Log($"player new pos{player.transform.position}");

            player.transform.SetParent(npc.transform);
            player.GetMounted();
            isCompleted = true;
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
        }
    }
}