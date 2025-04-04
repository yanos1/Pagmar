using UnityEngine;

namespace Triggers
{
    public abstract class Trigger : MonoBehaviour
    {
        protected bool isTriggered;

        public bool IsTriggered => isTriggered;
        
    }
}