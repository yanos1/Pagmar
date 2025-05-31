using System;
using Camera;
using DG.Tweening;
using Managers;
using MoreMountains.Feedbacks;
using ScripableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace NPC.NpcActions
{
    [Serializable]
    public class LinearMoveAction : MoveAction
    {
        // [SerializeField] private EventNames callbackEvent;
        // [SerializeField] private bool useCallback;
        [SerializeField] private PlayerSounds sounds;
        [SerializeField] private MMF_Player stepFeedbacks;
        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Walking);
            PerformMovement(npc);
        }

        protected override void PerformMovement(Npc npc)
        {
            Debug.Log($"moving npc in {duration} seconds");
            if (sounds is not null)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.walkSound, npc.transform.position);
            }
            stepFeedbacks?.PlayFeedbacks();
            npc.transform.DOMove(npc.transform.position + targetPosition, duration)
                .SetEase(easeType)
                .OnComplete(
                    () =>
                    {
                        isCompleted = true;
                        Debug.Log("Completed Movement!");
                    });

        }

        public override void UpdateAction(Npc npc)
        {
        }
    }


}