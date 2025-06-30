using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

namespace UI
{
    public static class UIUtilityFunctions
    {
        public static IEnumerator TransferNumberCoroutine(TextMeshProUGUI transferringText,
            TextMeshProUGUI receivingText, int amount,
            float transferDuration, Action onComplete)
        {
            float elapsedTime = 0f;
            int startValueTransferring = int.Parse(transferringText.text);
            int startValueReceiving = int.Parse(receivingText.text);
            int finalTransferringValue = startValueTransferring - amount;
            int finalReceivingValue = startValueReceiving + amount;

            while (elapsedTime < transferDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / transferDuration);

                // Smoothly interpolate the values
                int currentTransferringValue =
                    Mathf.RoundToInt(Mathf.Lerp(startValueTransferring, finalTransferringValue, progress));
                int currentReceivingValue =
                    Mathf.RoundToInt(Mathf.Lerp(startValueReceiving, finalReceivingValue, progress));

                // Update the text fields
                transferringText.text = currentTransferringValue.ToString();
                receivingText.text = currentReceivingValue.ToString();

                yield return null; // Wait for the next frame
            }

            // Ensure final values are set precisely at the end
            transferringText.text = finalTransferringValue.ToString();
            receivingText.text = finalReceivingValue.ToString();
            onComplete?.Invoke();
        }

        private static IEnumerator SpawnParticlesCoroutine(ParticleSystem transferParticles, Vector3 startPos,
            Vector3 targetPos, float transferDuration)
        {
            // Play the particle system
            transferParticles.Play();

            float elapsedTime = 0f;
            while (elapsedTime < transferDuration)
            {
                elapsedTime += Time.deltaTime;

                // Get current particle positions
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[transferParticles.main.maxParticles];
                int particleCount = transferParticles.GetParticles(particles);

                // Move each particle towards the target position
                for (int i = 0; i < particleCount; i++)
                {
                    particles[i].position = Vector3.Lerp(startPos, targetPos, elapsedTime / transferDuration);
                }

                // Update particles in the system
                transferParticles.SetParticles(particles, particleCount);

                yield return null; // Wait for the next frame
            }

            // Stop the particle system when transfer is complete
            transferParticles.Stop();
        }
        
        
        public static Vector3 GetWorldPositionFromUI(RectTransform uiElement, Canvas canvas, UnityEngine.Camera camera)
        {
            // Get the position of the UI element in screen space
            Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, uiElement.position);

            // Convert the screen space position to world space
            Vector3 worldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiElement, screenPos, camera, out worldPos);

            return worldPos;
        }
    }
}