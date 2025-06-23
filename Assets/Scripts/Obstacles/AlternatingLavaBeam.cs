using System;
using System.Collections;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using SpongeScene;
using Terrain.Environment;

namespace Obstacles
{
    using UnityEngine;

    public class AlternatingLavaBeam : MonoBehaviour, IKillPlayer, IResettable
    {
        [SerializeField] protected float warningTime;
        [SerializeField] protected float delayBeforeFirstBeam;
        [SerializeField] protected float offTime = 5f;
        [SerializeField] protected float onTime = 2.5f;
        [SerializeField] protected EventNames onFinished;
        protected Collider2D col;
        protected Coroutine toggleCoroutine;
        protected bool isOff = true;
        protected bool isFirst = true;
        [SerializeField] protected MMF_Player startFeedbacks;
        public virtual void Start()
        {
            col = GetComponent<Collider2D>();
        }

        public virtual void StartBeam()
        {
             this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        public virtual IEnumerator ToggleBeam()
        {
            yield return new WaitForSeconds(delayBeforeFirstBeam);
            while (true)
            {

                if (isOff)
                {
                    startFeedbacks?.PlayFeedbacks();
                    if (isFirst)
                    {
                        isFirst = false;
                        yield return new WaitForSeconds((offTime - warningTime) /2);
                    }
                    else
                    {
                        yield return new WaitForSeconds((offTime - warningTime));
                    }
                    
            
                    yield return new WaitForSeconds(warningTime);
                }
                else
                {
                    yield return new WaitForSeconds(onTime);
                }

                isOff = !isOff;
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

        public virtual void ResetToInitialState()
        {
            StopAllCoroutines();
        
        
            col.enabled =false;


            isFirst = true;
            isOff = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IBreakable>() is { } breakable)
            {
                breakable.OnBreak();
            } 
        }
    }

}

