using System.Collections;
using FMODUnity;
using UnityEngine;
using Managers;
using Player;

namespace Terrain.Environment
{
    public class ArenaElevetor : MovingPlatform
    {
        [SerializeField] private Transform center;
        [SerializeField] private GameObject bars;
        [SerializeField] private MusicType ArenaMusic;

        private bool triggered = false;

        public override void Update()
        {
            base.Update();
        }

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
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.ChangeMusic, ArenaMusic);
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, null);
            StartCoroutine(LiftBarsWhenElevatorDone());
        }

        private IEnumerator LiftBarsWhenElevatorDone()
        {
            yield return new WaitUntil(() => isMoving == false);

            // PlayMoveSound();
            // Lerp bars upward
            Vector3 startPos = bars.transform.position;
            Vector3 endPos = startPos + Vector3.up * 20f; // lift 3 units up (adjust as needed)
            float duration = 3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                bars.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            // StopMoveSound();
            bars.transform.position = endPos;
        }


        public override void ResetToInitialState()
        {
            return; // this platform only moves once in the game. no need to return to origina lstate.
        } 
    }
}