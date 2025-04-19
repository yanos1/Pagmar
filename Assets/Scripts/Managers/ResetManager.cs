using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Triggers;
using UnityEngine;
using UnityEngine.Windows;

namespace Managers
{
  

    public class ResetManager : MonoBehaviour
    {
        private List<IResettable> resettables = new List<IResettable>();
        private Checkpoint lastCheckPoint;


      
        
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.Die, ResetAll);
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, FindResetAblesInScene);
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.Die, ResetAll);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, FindResetAblesInScene);
        }

       

        public void ResetAll(object obj)
        {
            foreach (var r in resettables)
            {
                r.ResetToInitialState();
            }
            
            RestoreCheckPoint();
        }

        public void UpdateCheckPoint(Checkpoint checkpoint)
        {
            lastCheckPoint = checkpoint;
        }

        public void AddResettable(IResettable resettable)
        {
            resettables.Add(resettable);
        }
        
        private void FindResetAblesInScene(object obj)
        {
            print("find restables 123");
            resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToList();
            print($"resetablees size {resettables.Count}");
            foreach (var r in resettables)
            {

                print($" 123 {r.ToString()}");
            }
        }
        
        private void RestoreCheckPoint()
        {
            lastCheckPoint.RestoreCheckpointState();
        }
    }


}