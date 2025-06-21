using System;
using FMOD.Studio;
using Managers;
using Mono.Cecil;
using UnityEngine;
using EventReference = FMODUnity.EventReference;

namespace Terrain.Environment
{
    public class AmbienceZone : MonoBehaviour
    {
        [SerializeField] private EventReference amb;
        private EventInstance ambRef;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ambRef = CoreManager.Instance.AudioManager.CreateEventInstance(amb);
                ambRef.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform)); // Sets (x, y, z=0)

                ambRef.start();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ambRef.stop(STOP_MODE.ALLOWFADEOUT);
                ambRef.release();
            }
        }
    }
}