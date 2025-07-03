using System;
using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;


namespace Triggers
{
    public class PlayAmbienceTemporarly : MonoBehaviour
    {
        [SerializeField] private EventReference ambience;
        private EventInstance e;

        public void StartAmbience()
        {
            e = CoreManager.Instance.AudioManager.CreateEventInstance(ambience);
            e.start();
        }

        public void StopAmbience()
        {
            e.stop(STOP_MODE.ALLOWFADEOUT);
            e.release();
        }
    }
    
}