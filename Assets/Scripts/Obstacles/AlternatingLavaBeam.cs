using System;
using System.Collections;
using Interfaces;
using Managers;
using SpongeScene;
using Terrain.Environment;

namespace Obstacles
{
    using UnityEngine;

    public class AlternatingLavaBeam : MonoBehaviour, IKillPlayer, IResettable
    {
        [SerializeField] private SpriteRenderer beamSprite;
        [SerializeField] private SpriteRenderer warning;
        [SerializeField] private float warningTime;
        [SerializeField] private float delayBeforeFirstBeam;
        [SerializeField] private float offTime = 5f;
        [SerializeField] private float onTime = 2.5f;
        [SerializeField] private EventNames onFinished;
        private Collider2D col;
        private Coroutine toggleCoroutine;
        private bool isOff = true;
        private bool isFirst = true;
        private void Start()
        {
         
            beamSprite = GetComponent<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            
        }

        public void StartBeam()
        {
             this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        private IEnumerator ToggleBeam()
        {
            yield return new WaitForSeconds(delayBeforeFirstBeam);
            while (true)
            {

                if (isOff)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        yield return new WaitForSeconds((offTime - warningTime) /2);
                    }
                    else
                    {
                        yield return new WaitForSeconds((offTime - warningTime));
                    }
                    StartCoroutine(UtilityFunctions.FadeImage(warning, 0.6f, 0, warningTime, null));
                    
            
                    yield return new WaitForSeconds(warningTime);
                }
                else
                {
                    yield return new WaitForSeconds(onTime);
                }

                isOff = !isOff;
                beamSprite.enabled = !beamSprite.enabled;
                col.enabled = !col.enabled;
                if (onFinished is not EventNames.None && col.enabled == false)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onFinished, null);
                }
            }
        }

        public bool IsDeadly()
        {
            return true;
        }

        public void ResetToInitialState()
        {
            StopAllCoroutines();
            var c = warning.color;
            c.a = 0;
            warning.color = c;
            beamSprite.enabled = false;
            col.enabled =false;


            isFirst = true;
            isOff = true;
        }
    }

}

