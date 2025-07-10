using System.Collections;
using UnityEngine;
using Managers;
using Player;
using WaitForSeconds = UnityEngine.WaitForSeconds;

namespace Terrain.Environment
{
    public class ArenaElevetor : MovingPlatform
    {
        [SerializeField] private Transform center;

        private bool triggered = false;
        public override void OnCollisionEnter2D(Collision2D c)
        {
            // base.OnCollisionEnter2D(c);
            if (c.gameObject.GetComponent<PlayerManager>() is { } playerManager)
            {   
                if (!isMoving && !triggered)
                {
                    triggered = true;
                    
                    StartCoroutine(DelayedMove(playerManager));
                }
            }
        }

        private IEnumerator DelayedMove(PlayerManager playerManager)
        {
            yield return new WaitForSeconds(1f);
            playerManager.StopAllMovement();
            playerManager.DisableInput();
            yield return new WaitForSeconds(0.6f);
            MovePlatformAndTakeInput(playerManager);
        }

        public void MovePlatformAndTakeInput(PlayerManager playerManager)
        {
            playerManager.transform.position = center.position;
            MovePlatformExternally();
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, null);
        }

        public override void ResetToInitialState()
        {
            return; // this platform only moves once in the game. no need to return to origina lstate.
        } 
    }
}