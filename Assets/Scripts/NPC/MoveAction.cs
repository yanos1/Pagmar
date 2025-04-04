using System;

namespace NPC
{
    using UnityEngine;
    using DG.Tweening;

    [Serializable]
    public abstract class MoveAction : NpcAction
    {
        [SerializeField] protected Vector3 targetPosition;
        [SerializeField] protected float duration;
        [SerializeField] protected Ease easeType;

        public override void StartAction(Npc npc)
        {
            PerformMovement(npc);
        }

        protected abstract void PerformMovement(Npc npc);
    }
}