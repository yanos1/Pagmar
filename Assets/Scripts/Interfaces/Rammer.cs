namespace Interfaces
{
    using UnityEngine;

    public abstract class Rammer : MonoBehaviour
    {
        public float CurrentForce { get; protected set; }

        public abstract void OnRam(float againstForce);
        public abstract void OnRammed(float fromForce);
    
        public abstract void ApplyKnockback(Vector2 direction, float force);

        public virtual void ResetForce()
        {
            CurrentForce = 0f;
        }
    }

}