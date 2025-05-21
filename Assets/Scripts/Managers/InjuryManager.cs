using System;
using UnityEngine;

namespace Managers
{
    public class InjuryManager : MonoBehaviour
    {

        public static InjuryManager Instance;
        [Range(0, 1)] public float injuryMagnitude = 0f;
        public CanvasGroup redVignetteCanvasGroup;
        public AudioSource injuryAudioSource;

        [Header("Settings")]
        public AnimationCurve vignetteCurve; // Curve to control vignette intensity over injuryMagnitude
        public AnimationCurve pitchCurve;    // Curve to control pitch over injuryMagnitude
        public AnimationCurve volumeCurve;   // Curve to control volume over injuryMagnitude

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            UpdateVisualFeedback();
            UpdateAudioFeedback();
        }

        void UpdateVisualFeedback()
        {
            if (redVignetteCanvasGroup)
            {
                float alpha = vignetteCurve.Evaluate(injuryMagnitude);
                redVignetteCanvasGroup.alpha = alpha;
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
            injuryMagnitude = Mathf.Clamp01(injuryMagnitude + amount);
        }

        public void Heal(float amount)
        {
            injuryMagnitude = Mathf.Clamp01(injuryMagnitude - amount);
        }
    }

}