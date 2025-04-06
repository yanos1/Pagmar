using System;
using Managers;
using Unity.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private void Start()
        {
            CoreManager.Instance.Player = this;
        }
    }
}