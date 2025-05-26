using FMODUnity;
using UnityEngine;

namespace ScripableObjects
{
    [CreateAssetMenu(fileName = "LevelSounds", menuName = "Audio/LevelSounds", order = 1)]
    public class LevelSounds : ScriptableObject
    {
        public EventReference ambienceUpper;
        public EventReference ambienceLower;

        // You can also add volume settings per sound if needed
        [Range(0f, 1f)] public float jumpVolume = 1f;
        [Range(0f, 1f)] public float landVolume = 1f;
        [Range(0f, 1f)] public float damageVolume = 1f;
        [Range(0f, 1f)] public float deathVolume = 1f;
        [Range(0f, 1f)] public float attackVolume = 1f;
    }

}