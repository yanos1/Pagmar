﻿namespace Interfaces
{
    using UnityEngine;

    public abstract class Rammer : MonoBehaviour
    {
        public float CurrentForce { get; protected set; }
        public float MaxForce { get; protected set; }
        public bool IsCharging { get; protected set; }

        public abstract void OnRam(Vector2 ramDirNegative, float againstForce);
        public abstract void OnRammed(float fromForce, Vector3 collisionPoint);
        public abstract void OnTie(float fromForce, Vector3 collisionPoint);
    
        public abstract void ApplyKnockback(Vector2 direction, float force);

        public virtual void ResetForce()
        {
            CurrentForce = 0f;
        }
    }

}