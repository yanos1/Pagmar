using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {

        private bool inCutScene = false;
        private bool allowPlayerInput = true;
        
        public bool InCutScene => inCutScene;
        public bool AllowPlayerInput => allowPlayerInput;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, OnEnterCutScene);
            // CoreManager.Instance.EventManager.AddListener(EventNames.AllowCutSceneInput, AlowCutSceneInput);
            // CoreManager.Instance.EventManager.AddListener(EventNames.EndCutScene, OnEndCutScene);
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);
            // CoreManager.Instance.EventManager.RemoveListener(EventNames.AllowCutSceneInput, AlowCutSceneInput);
            // CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, OnEndCutScene);
        }
        
        public void OnEndCutScene(object obj)
        { 
            inCutScene = false;
            allowPlayerInput = true;
        }
        
        public void AlowCutSceneInput()
        {
            print("allow player input from game manager!!!!!!");
            allowPlayerInput = true;
        }
        
        private void OnEnterCutScene(object obj)
        {
            print("Enter cut scene cut input!!!!!!");

            inCutScene = true;
            allowPlayerInput = false;
        }
    }
}