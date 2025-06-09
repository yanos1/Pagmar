using System;
using System.Collections;
using Interfaces;
using Managers;
using SpongeScene;
using Terrain.Environment;
using UnityEngine;

namespace Obstacles
{
    public class MovingAlternatingLavaBeam : AlternatingLavaBeam
    {
        
        [SerializeField] private float targetXPosition;
        private bool hasReachedTarget;
        private Vector3 startingPos;
        
        public override void StartBeam()
        {
            startingPos = transform.position;
            this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        private IEnumerator ToggleBeam()
        {
            yield return new WaitForSeconds(delayBeforeFirstBeam);

            while (!hasReachedTarget)
            {
                if (isOff)
                {
                    startFeedbacks?.PlayFeedbacks();
                    yield return new WaitForSeconds(offTime - warningTime);
                    StartCoroutine(UtilityFunctions.FadeImage(warning, 0.6f, 0, warningTime, null));
                    yield return new WaitForSeconds(warningTime);
                }
                else
                {
                    yield return new WaitForSeconds(onTime);
                }

                isOff = !isOff;

                // Only turn ON if still within target
                if (!isOff && transform.position.x >= targetXPosition)
                {
                    hasReachedTarget = true;
                    beamSprite.enabled = false;
                    col.enabled = false;
                    yield break;
                }

                beamSprite.enabled = !isOff;
                col.enabled = !isOff;

                if (onFinished is not EventNames.None && col.enabled == false)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onFinished, null);
                }
                float beamAdvanceDistance = 4.3f;
                Vector3 futurePos = transform.position + Vector3.right * beamAdvanceDistance;
                warning.transform.position = new Vector3(futurePos.x, CoreManager.Instance.Player.transform.position.y + 2.3f, 0);
                transform.position = futurePos;

            }
        }
        
        public override void ResetToInitialState()
        {
           base.ResetToInitialState();
           transform.position = startingPos;
           hasReachedTarget = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IBreakable>() is { } breakable)
            {
                breakable.OnBreak();
            }

            if (other.CompareTag("Rock"))
            {
                Debug.Log("Found rock");
                other.gameObject.SetActive(false);
            }
        }
    }
}
