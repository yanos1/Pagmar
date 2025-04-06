using System;
using Camera;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace NPC.NpcActions
{
    [Serializable]
    public class LinearMoreAction : MoveAction
    {
        // [SerializeField] private EventNames callbackEvent;
        // [SerializeField] private bool useCallback;
        [SerializeField] private UnityEvent callBack;
        protected override void PerformMovement(Npc npc)
        {
            npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType)
                .OnComplete(
                    () =>
                    {
                        isCompleted = true;
                        callBack?.Invoke();
                    });

        }

        public override void UpdateAction(Npc npc)
        {
        }

        public void Shake()
        {
            CameraManager.Instance.ShakeCamera(0.1f,0.1f);
            
        }
    }


}