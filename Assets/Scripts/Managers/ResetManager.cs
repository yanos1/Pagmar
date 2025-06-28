using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = UnityEngine.Input;

namespace Managers
{
    public class ResetManager : MonoBehaviour
    {
        private List<IResettable> resettables = new List<IResettable>();
        private Checkpoint lastCheckPoint;

        private bool isResetting = false;

        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.Die, HandlePlayerDie);
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, FindResetAblesInScene);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.Die, HandlePlayerDie);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, FindResetAblesInScene);
            StopAllCoroutines();
        }

        private void HandlePlayerDie(object obj)
        {
            if (isResetting) return;
            StartCoroutine(ResetAll(obj));
        }

        public IEnumerator ResetAll(object obj, bool restoreCheckpoint = true)
        {
            if (isResetting) yield break;
            isResetting = true;

            Debug.Log("Reset All triggered!");
            CoreManager.Instance.UiManager.ShowLoadingScreen();
            CoreManager.Instance.Player.DisableInput();

            while (!CoreManager.Instance.UiManager.IsFadeInFinished())
            {
                yield return null;
            }

            foreach (var r in resettables)
            {
                r.ResetToInitialState();
            }

            CoreManager.Instance.UiManager.HideLoadingScreen();
        

            if (restoreCheckpoint)
            {
                RestoreCheckPoint();
            }

            isResetting = false;
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

            // Add from active scene
            resettables.AddRange(FindObjectsOfType<MonoBehaviour>().OfType<IResettable>());

            // Add from PersistentScene
            Scene persistentScene = SceneManager.GetSceneByName("PersistentScene");
            if (!persistentScene.IsValid())
            {
                Debug.LogError("PersistentScene not found or not loaded.");
                return;
            }

            foreach (GameObject rootObj in persistentScene.GetRootGameObjects())
            {
                var found = rootObj.GetComponentsInChildren<MonoBehaviour>(true)
                                   .OfType<IResettable>();
                resettables.AddRange(found);
            }
        }

        private void RestoreCheckPoint()
        {
            lastCheckPoint?.RestoreCheckpointState();
        }
    }
}