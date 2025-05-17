using System;
using Managers;
using UnityEngine;

namespace Terrain.Environment
{
    public class ShrinkCrumblinggPlatform : CrumblingPlatform
    {
        private Vector3 startingScale;
        
        
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.GayserFinished, OnGayserFinished);
            startingScale = transform.localScale;
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.GayserFinished, OnGayserFinished);
        }

        private void OnGayserFinished(object obj)
        {
            print("Shrink !!");
            var current = transform.localScale;
            current.x = Mathf.Max(0, current.x - 0.3f);
            transform.localScale = current;
            if (Mathf.Approximately(transform.localScale.x, 0f))
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            transform.localScale = startingScale;
        }
    }
}