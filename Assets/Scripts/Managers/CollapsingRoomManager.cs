using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CollapsingRoomManager : MonoBehaviour
    {
        [SerializeField] private List<MMF_Player> collapsefeebacks;
        private int currentIndex = 0;

        private void Start()
        {
            collapsefeebacks[currentIndex]?.PlayFeedbacks();
        }

        public void InvokeNextFeedbacks()
        {
            collapsefeebacks[currentIndex]?.StopFeedbacks();
            if (++currentIndex == collapsefeebacks.Count)
            {
                return;
            }
            collapsefeebacks[currentIndex]?.PlayFeedbacks();
        }

        private void OnDestroy()
        {
            CinemachineImpulseManager.Instance.Clear(); // stop shaking of camera.
        }
    }
}