using System;
using Interfaces;
using UnityEngine;

namespace Managers
{
    public class InjuryManager : MonoBehaviour, IResettable
    {
        public static InjuryManager Instance;

        [Range(0, 1)] public float injuryMagnitude = 0f;
        public CanvasGroup redVignetteCanvasGroup;
        public AudioSource injuryAudioSource;

        [Header("Settings")]
        public AnimationCurve vignetteCurve; // Curve to control vignette intensity over injuryMagnitude

        public AnimationCurve pitchCurve; // Curve to control pitch over injuryMagnitude
        public AnimationCurve volumeCurve; // Curve to control volume over injuryMagnitude

        [Header("Healing Settings")] public bool enablePassiveHealing = true;
        public float passiveHealingRate = 0.13f; // heals 12% per second
        public float healingDelay = 2f; // time after damage before healing begins

        private float lastDamageTime;


        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            UpdateVisualFeedback();
            UpdateAudioFeedback();
            HandlePassiveHealing();
        }

        void UpdateVisualFeedback()
        {
            if (redVignetteCanvasGroup)
            {
                redVignetteCanvasGroup.alpha = injuryMagnitude / 7;
            }
        }

        void UpdateAudioFeedback()
        {
            if (injuryAudioSource)
            {
                injuryAudioSource.pitch = pitchCurve.Evaluate(injuryMagnitude);
                injuryAudioSource.volume = volumeCurve.Evaluate(injuryMagnitude);
            }
        }


        public void ApplyDamage(float amount)
        {
            lastDamageTime = Time.time; // reset healing delay
            // if (amount + injuryMagnitude > 0.9)
            // {
            //     CoreManager.Instance.Player.Die();
            //     return;
            // }
            injuryMagnitude = Mathf.Clamp01(injuryMagnitude + amount);
        }


        public void Heal(float amount = 1)
        {
            injuryMagnitude = Mathf.Clamp01(injuryMagnitude - amount);
        }

        void HandlePassiveHealing()
        {
            if (!enablePassiveHealing) return;

            // Wait for delay after last damage
            if (Time.time - lastDamageTime >= healingDelay && injuryMagnitude > 0f)
            {
                Heal(passiveHealingRate * Time.deltaTime);
            }
        }

        public void ResetToInitialState()
        {
            injuryMagnitude = 0f;
            lastDamageTime = 0f;

            if (redVignetteCanvasGroup)
                redVignetteCanvasGroup.alpha = 0f;

            if (injuryAudioSource)
            {
                injuryAudioSource.pitch = pitchCurve.Evaluate(0f);
                injuryAudioSource.volume = volumeCurve.Evaluate(0f);
            }
        }
    }
}