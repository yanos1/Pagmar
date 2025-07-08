using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameAmbience", menuName = "Audio/GameAmbience", order = 1)]
    public class GameAmbience : ScriptableObject
    {
        [SerializeField] private List<AmbienceReference> _ambiences;

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
    public class AmbienceReference
    {
        public AmbienceType type;
        public EventReference ambience;

        public AmbienceReference(AmbienceType type, EventReference ambience)
        {
            this.type = type;
            this.ambience = ambience;
        }
    }
    
    public class AmbienceInstance
    {
        public AmbienceType type;
        public EventInstance ambience;

        public AmbienceInstance(AmbienceType type, EventInstance ambience)
        {
            this.type = type;
            this.ambience = ambience;
        }
    }
}