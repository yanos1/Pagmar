using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{

    [Serializable]
    class PlayerPosition
    {
        public int scene;
        public Vector3 position;
    }
    public class PlayerPositionManager : MonoBehaviour
    { 
        [SerializeField] private List<PlayerPosition> positionPerScene;


        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, OnSceneChange);
        }
        
        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, OnSceneChange);
        }

        private void OnSceneChange(object obj)
        {
            // if (obj is int newScene)
            // {
            //     CoreManager.Instance.Player.transform.position = positionPerScene[newScene].position;
            // }
        }
    }
}