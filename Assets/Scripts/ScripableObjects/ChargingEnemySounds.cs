using FMODUnity;
using UnityEngine;

namespace ScripableObjects
{
    [CreateAssetMenu(fileName = "ChargingEnemySounds", menuName = "Audio/ChargingEnemySounds", order = 1)]
    public class ChargingEnemySounds : ScriptableObject
    {
        public EventReference walkSound;
        public EventReference chargeSound;
        public EventReference damagedSound;
        public EventReference deathSound;
        public EventReference hitSound;
        public EventReference wakeUpSound;
        public EventReference sleepSound;
        public EventReference ramWall;
        public EventReference loadCharge;
    }
}