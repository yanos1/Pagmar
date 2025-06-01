using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Atmosphere
{
    public class Shaker : MonoBehaviour
    {

        private void Start()
        {
            GetComponent<MMF_Player>()?.PlayFeedbacks();
        }
    }
}