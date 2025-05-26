using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using JetBrains.Annotations;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        
        
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }

        public EventInstance CreateEveneInstance(EventReference sound, [CanBeNull] string parameter, [CanBeNull] float parameterValue)
        {
           EventInstance instance = RuntimeManager.CreateInstance(sound);
           if (parameter is not null)
           {
               instance.setParameterByName(parameter, parameterValue);
           }

           return instance;

        }
        
        public void SetGlobalParameter(string parameter, float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameter, value);

        }
    }
    
   
}