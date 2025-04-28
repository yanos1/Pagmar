using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            resettables.Clear();
            resettables.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToList());

            var sceneName = "PersistentScene";
            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            if (!targetScene.IsValid())
            {
                Debug.LogError($"Scene '{sceneName}' not found or not loaded.");
                return;
            }

            // Loop through all root GameObjects in the scene
            foreach (GameObject rootObj in targetScene.GetRootGameObjects())
            {
                // Get all MonoBehaviours on this GameObject and its children that implement IResettable
                IResettable[] found = rootObj.GetComponentsInChildren<MonoBehaviour>(true)
                    .OfType<IResettable>()
                    .ToArray();

                resettables.AddRange(found);
            }
        }

        private void RestoreCheckPoint()
        {
            lastCheckPoint.RestoreCheckpointState();
        }
    }


}