using System;
using System.Collections.Generic;
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
        public EventReference damagedHardSound;
        public EventReference deathSound;
        public EventReference attackSound;
        public EventReference clashSound;
        public EventReference laugh;
        public EventReference gasp;
        public EventReference wakeUp;
        public EventReference fallGasp;
        public EventReference playerSigh;
        public EventReference breath;
        public List<DamageTakenToSound> hornsDamagedPerLevel;


        public EventReference GethitSound(int currentDamage)
        {
                var match = hornsDamagedPerLevel.Find(pair => pair.damageTaken == currentDamage);
                Debug.Log("match {match}");
            if (match == null)
            {
                return default;
            }
            return match.sound;
        }
    }

    [Serializable]
    public class DamageTakenToSound
    {
        public int damageTaken;
        public EventReference sound;
    }

}