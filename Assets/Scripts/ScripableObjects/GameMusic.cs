using System;
using System.Collections.Generic;
using FMODUnity;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScripableObjects
{
    [CreateAssetMenu(fileName = "GameMusic", menuName = "Audio/GameMusic", order = 1)]
    public class GameMusic : ScriptableObject
    {
        [SerializeField] private List<Music> musics;

        /// <summary>
        /// Get an FMOD EventReference based on AmbienceType.
        /// </summary>
        public EventReference GetMusic(MusicType type)
        {
            foreach (var ambience in musics)
            {
                if (ambience.type == type)
                    return ambience.music;
            }

            Debug.LogWarning($"Music of type {type} not found.");
            return default;
        }
    }


    [Serializable]
    public class Music
    {
        public MusicType type;
        public EventReference music;
    }
}