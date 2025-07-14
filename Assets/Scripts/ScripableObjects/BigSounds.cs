
    using FMODUnity;
    using Managers;
    using UnityEngine;

    namespace ScripableObjects
    {
        [CreateAssetMenu(fileName = "BigSounds", menuName = "Audio/Big Sounds", order = 1)]
        public class BigSounds : ScriptableObject
        {
            public EventReference jumpSound;
            public EventReference landSound;
            public EventReference dashSound;
            public EventReference stepSound;

            public void PlayJump(Transform t)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(jumpSound, t.position);
            }
            public void PlayLand(Transform t)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(landSound, t.position);
            }
            public void PlayDash(Transform t)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(dashSound, t.position);
            }
            public void PlayStep(Transform t)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(stepSound, t.position);
            }
            
        }

    }
