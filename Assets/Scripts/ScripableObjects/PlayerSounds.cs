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
        public EventReference deathSound;
        public EventReference attackSound;
        public EventReference clashSound;
        public EventReference laugh;
        public EventReference gasp;
        public EventReference wakeUp;
        public EventReference fallGasp;
        public EventReference playerSigh;
        public EventReference breath;
        public List<StageToDamagedSound> hornsDamagedPerLevel;


        public EventReference GethitSound(PlayerStage currentStage)
        {
            return hornsDamagedPerLevel.Find(pair => pair.stage == currentStage).sound;
        }
    }

    [Serializable]
    public class StageToDamagedSound
    {
        public PlayerStage stage;
        public EventReference sound;
    }

}