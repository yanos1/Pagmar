using FMODUnity;
using UnityEngine;

namespace ScripableObjects
{
    [CreateAssetMenu(fileName = "PlayerSounds", menuName = "Audio/Player Sounds", order = 1)]
    public class PlayerSounds : ScriptableObject
    {
        public EventReference jumpSound;
        public EventReference walkSound;
        public EventReference fallSound;
        public EventReference landSound;
        public EventReference heavyLandSound;
        public EventReference damageSound;
        public EventReference deathSound;
        public EventReference attackSound;

        // You can also add volume settings per sound if needed
        [Range(0f, 1f)] public float jumpVolume = 1f;
        [Range(0f, 1f)] public float landVolume = 1f;
        [Range(0f, 1f)] public float damageVolume = 1f;
        [Range(0f, 1f)] public float deathVolume = 1f;
        [Range(0f, 1f)] public float attackVolume = 1f;
    }

}