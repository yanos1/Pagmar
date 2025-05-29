using System;
using System.Collections.Generic;
using FMODUnity;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameAmbience", menuName = "Audio/GameAmbience", order = 1)]
    public class GameAmbience : ScriptableObject
    {
        [SerializeField] private List<Ambience> _ambiences;

        /// <summary>
        /// Get an FMOD EventReference based on AmbienceType.
        /// </summary>
        public EventReference GetAmbience(AmbienceType type)
        {
            foreach (var ambience in _ambiences)
            {
                if (ambience.type == type)
                    return ambience.ambience;
            }

            Debug.LogWarning($"Ambience of type {type} not found.");
            return default;
        }
    }

    [Serializable]
    public class Ambience
    {
        public AmbienceType type;
        public EventReference ambience;
    }
}