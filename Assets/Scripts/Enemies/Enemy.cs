using System;
using Interfaces;
using Managers;
using UnityEngine;

namespace Enemies
{
    public abstract class Enemy : Rammer, IResettable
    {
        protected Vector3 startingPos;

        public virtual void Start()
        {
            startingPos = transform.position;
        }

        public virtual void ResetToInitialState()
        {
            transform.position = startingPos;
        }
    }
}