using FMODUnity;
using UnityEngine;

namespace ScripableObjects
{
    [CreateAssetMenu(fileName = "MirrorCheckpointSounds", menuName = "Audio/MirrorCheckpointSounds", order = 1)]
    public class MirrorCheckpointSounds : ScriptableObject
    {
        public EventReference reachedMirrorSound;

    }

}