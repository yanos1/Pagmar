using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using JetBrains.Annotations;
using ScripableObjects;
using ScriptableObjects;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private GameAmbience _gameAmbience;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeAmbience, OnChangeAmbience);
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeAmbience, OnChangeAmbience);
        }


        private void OnChangeAmbience(object obj)
        {
            if (obj is AmbienceType ambienceType)
            {
                var ambience = _gameAmbience.GetAmbience(ambienceType);
                if (ambience.IsNull) return;

                var instance = RuntimeManager.CreateInstance(ambience);
                instance.start();
                
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

    public enum AmbienceType
    {
        None = 0,
        Upperground = 1,
        Underground = 2,
        Battle = 3,
    }
   
}