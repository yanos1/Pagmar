using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Managers
{
    public class InjuryFeedbacks : MonoBehaviour, IResettable
    {
        public static InjuryFeedbacks Instance;

        [Header("Visual Feedback")] public CanvasGroup screenColor;

        [Header("Vignette Settings")] public Volume postProcessingVolume; // Assign in Inspector
        private Vignette vignette;
        private Coroutine vignetteFadeCoroutine;
        private Coroutine screenColorFadeCoroutine;
        private UnityEngine.Camera mainCamera;

        private int totalHealth;
        private int currentDamageTaken;
        private float healInterval;

        private void Awake()
        {
            Instance = this;
            mainCamera = UnityEngine.Camera.main;
            if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out Vignette v))
            {
                vignette = v;
                vignette.intensity.Override(0f);
                vignette.active = false;
                print("vingerte found");
            }
        }

        public void Init(int totalHealth, float healInterval)
        {
            this.totalHealth = totalHealth;
            this.healInterval = healInterval;
            this.currentDamageTaken = 0; // Start at 0 health
        }

        public void UpdateVisualFeedback(bool hardEffect = false)
        {
            if (screenColor)
            {

                screenColor.alpha = hardEffect ? 0.1f : 0.05f;
                print($"screen color alpha is {screenColor.alpha}");
                if (screenColorFadeCoroutine != null)
                    StopCoroutine(screenColorFadeCoroutine);

                screenColorFadeCoroutine = StartCoroutine(FadeScreenColorOut(healInterval));
            }
        }


        private void UpdateVignetteEffect()
        {
            if (currentDamageTaken == totalHealth && vignette != null)
            {
                print("activate vingate");

                if (vignetteFadeCoroutine != null)
                    StopCoroutine(vignetteFadeCoroutine);

                vignetteFadeCoroutine = StartCoroutine(AnimateVignetteEffect(0.6f, 0.5f, healInterval));
            }
        }


        public void ApplyDamage(int amount)
        {
            currentDamageTaken = Mathf.Min(totalHealth, currentDamageTaken + amount);
            UpdateVignetteEffect();
        }

        public void Heal(int amount)
        {
            currentDamageTaken = Mathf.Max(0, currentDamageTaken - amount);
            if (vignetteFadeCoroutine is not null)
            {
                vignette.intensity.value = 0f;
                vignette.active = false;
                StopCoroutine(vignetteFadeCoroutine);
                vignetteFadeCoroutine = null;

            }

            if (screenColorFadeCoroutine is not null)
            {
                screenColor.alpha = 0f;
                StopCoroutine(screenColorFadeCoroutine);
                screenColorFadeCoroutine = null;
            }
        }

        public void ResetToInitialState()
        {
            currentDamageTaken = 0;

            if (screenColor)
                screenColor.alpha = 0f;
            if (vignette != null)
            {            
                vignette.active = false;
            }

        }

        private IEnumerator AnimateVignetteEffect(float targetIntensity, float lerpDuration, float totalDuration)
        {
            vignette.active = true;
            float initialIntensity = vignette.intensity.value;
            float time = 0f;

            // Lerp to targetIntensity over lerpDuration
            while (time < lerpDuration)
            {
                time += Time.deltaTime;
                float t = time / lerpDuration;
                vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, t);
                yield return null;
            }

            vignette.intensity.value = targetIntensity;

            // Hold for 3 seconds
            float timer = 3f;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                if (currentDamageTaken != totalHealth)
                {
                    break;
                }
                yield return null;
            }

            // Fade to 0 over remaining time
            float fadeDuration = Mathf.Max(0f, totalDuration - 3f);
            time = 0f;
            float startIntensity = vignette.intensity.value;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
                vignette.intensity.value = Mathf.Lerp(startIntensity, 0f, t);
                yield return null;
            }

            vignette.intensity.value = 0f;
            vignette.active = false;
            vignetteFadeCoroutine = null;
        }



        private IEnumerator FadeScreenColorOut(float duration)
        {
            float holdTime = 3f;
            float fadeDuration = Mathf.Max(0f, duration - holdTime);

            yield return new WaitForSeconds(holdTime);

            float startAlpha = screenColor.alpha;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeDuration;
                screenColor.alpha = Mathf.Lerp(startAlpha, 0f, t);
                yield return null;
            }

            screenColor.alpha = 0f;
            screenColorFadeCoroutine = null;
        }
        
        public static Vector3 GetTopLeftWorld(UnityEngine.Camera cam, float planeZ = 0f)
        {
            // ── ORTHOGRAPHIC branch ────────────────────────────────────────────
            if (cam.orthographic)
            {
                float vertExtent  = cam.orthographicSize;
                float horizExtent = vertExtent * cam.aspect;

                Vector3 camPosOnPlane = cam.transform.position;
                if (Mathf.Abs(cam.transform.forward.z) > 0.01f)
                {
                    float dist = (planeZ - camPosOnPlane.z) / cam.transform.forward.z;
                    camPosOnPlane += cam.transform.forward * dist;  
                }
                Vector3 corner = camPosOnPlane
                                 + (-cam.transform.right * horizExtent)
                                 + ( cam.transform.up    * vertExtent);
                corner.z = planeZ;
                return corner;
            }

            Ray   ray = cam.ViewportPointToRay(new Vector3(0f, 1f, 0f));
            float t   = (planeZ - ray.origin.z) / ray.direction.z;        // param along the ray
            Debug.Log(ray.origin + ray.direction * t);
            return ray.origin + ray.direction * t;
        }


        
    }
}
