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
        [SerializeField]
        private GameAmbience _gameAmbience;

        private EventInstance currentAmbience;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, StopOldScneeSounds);
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, StopOldScneeSounds);

        }

        private void StopOldScneeSounds(object obj)
        {
            currentAmbience.stop(STOP_MODE.IMMEDIATE);
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
        
      


        public EventInstance CreateEveneInstance(EventReference sound, [CanBeNull] string parameter, [CanBeNull] float parameterValue)
        {
           EventInstance instance = RuntimeManager.CreateInstance(sound);
           if (parameter is not null)
           {
               instance.setParameterByName(parameter, parameterValue);
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
   
}