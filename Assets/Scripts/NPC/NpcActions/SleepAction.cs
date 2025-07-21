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

        private FMOD.Studio.EventInstance snoreInstance;

        public override void StartAction(Npc npc)
        {
            base.StartAction(npc);
            npc.SetState(NpcState.Sleeping);
            Debug.Log("play sleep sound");

            CoreManager.Instance.Runner.StartCoroutine(UtilityFunctions.WaitAndInvokeAction(2f,
                () =>
                {
                    snoreInstance = CoreManager.Instance.AudioManager.CreateEventInstance(snoring);
                    snoreInstance.set3DAttributes(RuntimeUtils.To3DAttributes(npc.transform.position));
                    snoreInstance.start();
                    Debug.Log("play snoring!!");
                    CoreManager.Instance.AudioManager.RegisterSoundToStopAtTheEndOfScene(snoreInstance);
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

            // Stop and release the FMOD event instance
            snoreInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            snoreInstance.release();

            afterActionCallback?.Invoke();
        }
    }
}