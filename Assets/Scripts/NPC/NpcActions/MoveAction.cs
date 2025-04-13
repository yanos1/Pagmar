using System;
using DG.Tweening;
using UnityEngine;

namespace NPC.NpcActions
{
    [Serializable]
    public abstract class MoveAction : NpcAction
    {
        [SerializeField] protected Vector3 targetPosition;
        [SerializeField] protected float duration;
        [SerializeField] protected Ease easeType;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
        }

        public override void ResetAction(Npc npc)
        {
            base.ResetAction(npc);
        }

        protected abstract void PerformMovement(Npc npc);
    }
}