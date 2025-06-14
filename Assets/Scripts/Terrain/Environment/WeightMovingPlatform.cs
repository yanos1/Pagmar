﻿using System.Collections;
using UnityEngine;
using Managers;
using Player;

namespace Terrain.Environment
{
    public class WeightMovingPlatform : MovingPlatform
    {
        [SerializeField] private Transform center;

        private void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.GetComponent<PlayerManager>() is { } playerManager)
            {
                if (!hasMoved)
                {
                    StartCoroutine(DelayedMove(playerManager));
                }
            }
        }

        private IEnumerator DelayedMove(PlayerManager playerManager)
        {
            yield return new WaitForSeconds(1.6f);
            MovePlatformAndTakeInput(playerManager);
        }

        public void MovePlatformAndTakeInput(PlayerManager playerManager)
        {
            playerManager.transform.position = center.position;
            playerManager.StopAllMovement();
            MovePlatformExternally();
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, null);
        }

        public override void ResetToInitialState()
        {
            return; // this platform only moves once in the game. no need to return to origina lstate.
        } 
    }
}