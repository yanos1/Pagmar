using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {

        private bool inCutScene = false;
        private bool IsGameRunning = false;
        public bool InCutScene => inCutScene;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, OnEnterCutScene);
            // CoreManager.Instance.EventManager.AddListener(EventNames.AllowCutSceneInput, AlowCutSceneInput);
            CoreManager.Instance.EventManager.AddListener(EventNames.EndCutScene, OnEndCutScene);
            StartCoroutine(AfkCheck());
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);
            // CoreManager.Instance.EventManager.RemoveListener(EventNames.AllowCutSceneInput, AlowCutSceneInput);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, OnEndCutScene);
        }

        public void OnEndCutScene(object obj)
        { 
            inCutScene = false;
        }
        
        private void OnEnterCutScene(object obj)
        {
            inCutScene = true;
        }

        private IEnumerator AfkCheck()
        {
            while (true)
            {
                if (CoreManager.Instance.Player && Time.time - CoreManager.Instance.Player.GetComponent<PlayerMovement>().LastMoveTime > 140f) // 5 minutes
                {
                    if (ScenesManager.Instance.CurrentScene != 2) // main menu
                    {
                        ScenesManager.Instance.LoadMainMenu();
                    }

                }
                yield return new WaitForSeconds(60f);

            }
        }
    }
}