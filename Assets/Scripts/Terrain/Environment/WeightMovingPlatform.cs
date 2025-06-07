using System.Collections;
using UnityEngine;
using Managers;

namespace Terrain.Environment
{
    public class WeightMovingPlatform : MovingPlatform
    {
        [SerializeField] private Transform center;

        private void OnCollisionEnter2D(Collision2D c)
        {
            if (c.gameObject.GetComponent<PlayerMovement>() is { } playerMovement)
            {
                if (!hasMoved)
                {
                    StartCoroutine(DelayedMove(playerMovement));
                }
            }
        }

        private IEnumerator DelayedMove(PlayerMovement playerMovement)
        {
            yield return new WaitForSeconds(1.6f);
            MovePlatformAndTakeInput(playerMovement);
        }

        public void MovePlatformAndTakeInput(PlayerMovement playerMovement)
        {
            playerMovement.transform.position = center.position;
            MovePlatformExternally();
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, null);
        }
    }
}