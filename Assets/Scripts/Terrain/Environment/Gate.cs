using System;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class Gate : MonoBehaviour, IResettable
    {
        private Vector3 startingPos;

        [SerializeField] private MMF_Player openFeedbacks;
        [SerializeField] private MMF_Player openDontClose;

        private void Start()
        {
            startingPos = transform.position;
        }
        
        public void Open()
        {
            openFeedbacks?.PlayFeedbacks();
        }
        
        public void OpenDontClose()
        {
            openDontClose?.PlayFeedbacks();
        }
     


        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            transform.position = startingPos;
        }
    }
}