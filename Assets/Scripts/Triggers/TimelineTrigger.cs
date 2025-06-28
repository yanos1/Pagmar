using Managers;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

namespace Triggers
{
    public class TimelineTrigger : Trigger
    {
        [Header("Trigger Settings")] private int triggered = 0;
        [SerializeField] private int requiredTriggers;
        [SerializeField] private PlayableDirector cutScene;
        [SerializeField] private float delayBeforeStartCutscene;
        [SerializeField] private bool PlaycutSceneOnlyOnce;
        [SerializeField] private bool playOnAwake = false;

        public override void Start()
        {
            base.Start();
            if (playOnAwake)
            {
                StartTimeline();
            }
        }

        public override void OnTriggerEnter2D(Collider2D other) 
        {
            print($"trigged timeline by {other.gameObject.name}");
            if (!isTriggered && other.CompareTag(trigger) && ++triggered == requiredTriggers)
            {
                StartTimeline();
            }
        }

        private void StartTimeline()
        {
            isTriggered = true;
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(delayBeforeStartCutscene, () =>
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.EnterCutScene, cutScene.tag);
                cutScene.Play();
            }));
        }


        public override void ResetToInitialState()
        {
            if (!PlaycutSceneOnlyOnce)
            {
                base.ResetToInitialState();
            }
            triggered = 0;
            cutScene.Stop();
        }
    }
    
}