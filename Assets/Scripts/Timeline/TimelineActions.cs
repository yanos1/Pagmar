using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Timeline
{
    public class TimelineActions : MonoBehaviour
    {
        [SerializeField] private List<MMF_Player> shakes;
        private int currentShake = 0;

        public void PlayShake()
        {
            print("play shake from timeline");
            shakes[currentShake++].PlayFeedbacks();
        }
    }
}