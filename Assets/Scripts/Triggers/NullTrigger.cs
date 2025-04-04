using System;

namespace Triggers
{
    public class VoidTrigger :  Trigger
    {
        private void Awake()
        {
            isTriggered = true;
        }
    }
}