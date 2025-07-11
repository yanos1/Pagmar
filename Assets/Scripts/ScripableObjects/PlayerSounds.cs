using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

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
        public EventReference dashSound;
        public EventReference wallJumpSound;
        public EventReference damagedSound;
        public EventReference deathSound;
        public EventReference attackSound;
        public EventReference clashSound;

    }

}