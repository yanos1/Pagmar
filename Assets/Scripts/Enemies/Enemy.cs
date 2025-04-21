using System;
using Interfaces;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : MonoBehaviour, IResettable
    {
        protected Vector3 startingPos;

        public virtual void Start()
        {
            startingPos = transform.position;
        }

        public abstract void OnRam();
        public virtual void ResetToInitialState()
        {
            transform.position = startingPos;
        }

        public abstract bool IsDeadly();
    }
}