﻿using System;
using FMODUnity;
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
        [SerializeField] private EventReference openSound;

        private void Start()
        {
            startingPos = transform.position;
        }
        
        public void Open()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(openSound, transform.position);
            openFeedbacks?.PlayFeedbacks();
        }
        
        public void OpenDontClose()
        {
            CoreManager.Instance.AudioManager.PlayOneShot(openSound, transform.position);

            openDontClose?.PlayFeedbacks();
        }
     


        public void ResetToInitialState()
        {
            gameObject.SetActive(true);
            transform.position = startingPos;
        }
    }
}