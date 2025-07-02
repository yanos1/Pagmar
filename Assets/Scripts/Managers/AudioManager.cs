using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using JetBrains.Annotations;
using ScripableObjects;
using ScriptableObjects;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private GameAmbience _gameAmbience;
        [SerializeField] private GameMusic _gameMusic;

        private EventInstance currentAmbience;
        private EventInstance currentMusic;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeMusic, OnChangeMusic);
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, StopOldScneeSounds);
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeMusic, OnChangeMusic);

            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, StopOldScneeSounds);
        }

     
        public void StopAllSounds()
        {
            Bus masterBus = RuntimeManager.GetBus("bus:/");
            masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
        }

        private void StopOldScneeSounds(object obj)
        {
            currentAmbience.stop(STOP_MODE.IMMEDIATE);
        }

        private void OnChangeMusic(object obj)
        {
            if (obj is MusicType musicType)
            {
                var ambience = _gameMusic.GetMusic(musicType);
                if (ambience.IsNull) return;

                currentAmbience = RuntimeManager.CreateInstance(ambience);
                currentAmbience.start();
                
            }        
        }


        public void OnChangeAmbience(object obj)
        {
            if (obj is AmbienceType ambienceType)
            {
                var ambience = _gameAmbience.GetAmbience(ambienceType);
                if (ambience.IsNull) return;

                currentAmbience = RuntimeManager.CreateInstance(ambience);
                currentAmbience.start();
                
            }
        }

        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }
        
      


        public EventInstance CreateEventInstance(EventReference sound, [CanBeNull] string parameter = null, float? parameterValue = null)
        {
            EventInstance instance = RuntimeManager.CreateInstance(sound);

            if (!string.IsNullOrEmpty(parameter) && parameterValue.HasValue)
            {
                instance.setParameterByName(parameter, parameterValue.Value);
            }

            return instance;
        }

        
        public void SetGlobalParameter(string parameter, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameter, value);

        }
    }

    [Serializable]
    public enum AmbienceType
    {
        None = 0,
        Upperground = 1,
        Underground = 2,
        Battle = 3,
    }
    
    [Serializable]
    public enum MusicType
    {
        None = 0,
        Underground1 = 1,
        Underground2 = 2,
        Eruption = 3,
        ArenaMusic = 4,
        ChaseMusic = 5,
        Upperground1 = 6,
        Upperground2 = 7,
    }
   
}