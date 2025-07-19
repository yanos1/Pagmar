using System;
using System.Collections.Generic;
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
        [SerializeField] private List<AmbienceInstance> temporalAmbiencesPlaying = new();
        private List<EventInstance> soundsToStopAtTheEndOfScene = new();

        private EventInstance currentAmbience;
        private EventInstance currentMusic;
        
        private Bus masterBus;

        private void Awake()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
        }
        
        
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.AddListener(EventNames.ChangeMusic, OnChangeMusic);
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, StopOldScneeSounds);
            CoreManager.Instance.EventManager.AddListener(EventNames.PickUpFakeRune, StopOldScneeSounds);
            CoreManager.Instance.EventManager.AddListener(EventNames.StopMusic, OnChangeMusic);
        }
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeAmbience, OnChangeAmbience);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.ChangeMusic, OnChangeMusic);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PickUpFakeRune, StopOldScneeSounds);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, StopOldScneeSounds);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StopMusic, OnChangeMusic);

            
        }

     
        public void StopAllSounds()
        {
            Bus masterBus = RuntimeManager.GetBus("bus:/");
            masterBus.stopAllEvents(STOP_MODE.IMMEDIATE);
        }

        private void StopOldScneeSounds(object obj)
        {
            print("stop old scene sounds!");
            currentAmbience.stop(STOP_MODE.IMMEDIATE);
            currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
            currentAmbience.release();

        }

        private void OnChangeMusic(object obj)
        {
            currentMusic.getPlaybackState(out PLAYBACK_STATE state);
            print("try to stop music");
            if (state == PLAYBACK_STATE.PLAYING)
            {
                print("stop music !");
                currentMusic.stop(STOP_MODE.ALLOWFADEOUT);
                currentMusic.release();
            }
            if (obj is MusicType musicType)
            {
                print("try to start new music !");
                var music = _gameMusic.GetMusic(musicType);
                if (music.IsNull) return;
                print("start new music !");

                currentMusic = RuntimeManager.CreateInstance(music);
                currentMusic.start();
                
            }        
        }

        public void MuteAll()
        {
            masterBus.setMute(true);
        }

        public void UnmuteAll()
        {
            masterBus.setMute(false);
        }
        
        public void PauseAllAudio()
        {
            masterBus.setPaused(true);
        }

        public void ResumeAllAudio()
        {
            masterBus.setPaused(false);
        }
        
        public void AddTemporalAmbience(AmbienceType ambienceType, EventReference ambience)
        {
            var addedAmbience = CreateEventInstance(ambience);
            temporalAmbiencesPlaying.Add(new AmbienceInstance(ambienceType, addedAmbience));
            addedAmbience.start();
        }

        public void RemoveTemporalAmbience(AmbienceType ambienceType)
        {
            foreach (AmbienceInstance instance in temporalAmbiencesPlaying)
            {
                if (ambienceType == instance.type)
                {
                    instance.ambience.stop(STOP_MODE.ALLOWFADEOUT);
                    instance.ambience.release();
                }
            }
        }

        public void OnChangeAmbience(object obj)
        {
            
            if (obj is AmbienceType ambienceType)
            {
                currentAmbience.getPlaybackState(out PLAYBACK_STATE state);

                if (state == PLAYBACK_STATE.PLAYING)
                {
                    currentAmbience.stop(STOP_MODE.ALLOWFADEOUT);
                    currentAmbience.release();
                }
          
                var ambience = _gameAmbience.GetAmbience(ambienceType);
                if (ambience.IsNull) return;

                currentAmbience = RuntimeManager.CreateInstance(ambience);
                currentAmbience.start();
                
            }
        }

        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            if (!sound.IsNull)
            {
                RuntimeManager.PlayOneShot(sound, worldPos);
            }
        }

        public void RegisterSoundToStopAtTheEndOfScene(EventInstance soundRef)
        {
            soundsToStopAtTheEndOfScene.Add(soundRef);
        }

        public void StopRegisteredSoundEvents()
        {
            foreach (var sound in soundsToStopAtTheEndOfScene)
            {
                sound.stop(STOP_MODE.ALLOWFADEOUT);
                sound.release();
            }
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
            print($"set global parameter {parameter}");
            RuntimeManager.StudioSystem.setParameterByName(parameter, value);

        }
        
        public void SetMusicLocalParameter(string parameter, float value)
        {
            print($"Set local parameter '{parameter}' to {value}");
            currentMusic.setParameterByName(parameter, value);
        }
    }

    [Serializable]
    public enum AmbienceType
    {
        None = 0,
        Upperground = 1,
        Underground = 2,
        EarthQuake = 3,
        ForestTreeArea = 4,
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