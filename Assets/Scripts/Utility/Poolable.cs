using System;
using Managers;
using UnityEngine;

namespace Utility
{
    using UnityEngine;

    public abstract class Poolable : MonoBehaviour
    {
        private PoolEnum type;

        
        public PoolEnum Type { get; set; }
        public virtual void Initialize()
        {
        }

        public virtual void OnGetFromPool() { }

        public virtual void OnReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }

}