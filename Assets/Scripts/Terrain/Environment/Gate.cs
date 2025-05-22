using System;
using Interfaces;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Terrain.Environment
{
    public class Gate : MonoBehaviour,IResettable
    {
        [SerializeField] private MMF_Player feedbacks;
        private Vector3 startPos;
        private void Start()
        {
            startPos = transform.position;
        }

        public void ResetToInitialState()
        {
            transform.position = startPos;
        }

        public void Open()
        {
            feedbacks.PlayFeedbacks();
        }
    }
}