using FMODUnity;
using Managers;
using NPC.BigFriend;
using SpongeScene;
using UnityEngine;

namespace NPC.NpcActions
{
    using System;


    [Serializable]
    public class SleepAction : NpcAction
    {
        [SerializeField] private BigActions _actions;
        [SerializeField] private EventReference snoring;
        private bool displayingSleep = false;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Sleeping);
            Debug.Log("play sleep sound");
            CoreManager.Instance.Runner.StartCoroutine(UtilityFunctions.WaitAndInvokeAction(2f,
                () =>
                {
                    CoreManager.Instance.AudioManager.PlayOneShot(snoring, npc.transform.position);
                    Debug.Log("play snoring!!");
                }));
        }

        public override void UpdateAction(Npc npc)
        {
            if (npc.IsJumpedOn && !displayingSleep)
            {
                displayingSleep = true;
                _actions.ShowSleepHeallRequest();
            }
            else if (npc.IsJumpedOn == false)
            {
                displayingSleep = false;
            }
        }

        public override void ResetAction(Npc npc)
        {
            Debug.Log("big is waking up");
            isCompleted = false;
            afterActionCallback?.Invoke();
        }
    }
}