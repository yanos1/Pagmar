using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SpongeScene
{
    public static class UtilityFunctions
    {
        public static T[] ShuffleArray<T>(T[] array)
        {
            T[] copy = (T[])array.Clone();
            System.Random random = new System.Random();
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (copy[i], copy[j]) = (copy[j], copy[i]);
            }

            return copy;
        }


        public static void ShuffleArray<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int randIndex = Random.Range(0, list.Count);
                (list[i], list[randIndex]) = (list[randIndex], list[i]);
            }
        }

        public static IEnumerator FadeImage(Renderer renderer, float startValue, float endValue,
            float imageFadeDuration)
        {
            float elapsedTime = 0f;

            Color color = renderer.material.color;

            while (elapsedTime < imageFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(startValue, endValue, elapsedTime / imageFadeDuration);

                color.a = currentAlpha;

                renderer.material.color = color;

                yield return null;
            }

            color.a = endValue;
            renderer.material.color = color;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator MoveObjectOverTime(GameObject obj, Vector3 startingPos, Quaternion startingRotation,
            Vector3 endingPos, Quaternion endingRotation, float duration, Action onComplete = null)
        {
            float timeElapsed = 0;
            float percentageCompleted = 0;
            while (percentageCompleted < 1)
            {
                timeElapsed += Time.deltaTime;
                percentageCompleted = timeElapsed / duration;
                obj.transform.position = Vector3.Lerp(startingPos, endingPos, percentageCompleted);
                obj.transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, percentageCompleted);
                yield return null;
            }

            obj.transform.position = endingPos;
            obj.transform.rotation = endingRotation;

            // Invoke the callback if provided
            onComplete?.Invoke();
        }

        public static IEnumerator MoveObjectOverTime(GameObject obj, Vector3 startingPos, Quaternion startingRotation,
            Transform endingPos, Quaternion endingRotation, float duration, Action onComplete = null)
        {
            float timeElapsed = 0;
            float percentageCompleted = 0;
            while (percentageCompleted < 1)
            {
                timeElapsed += Time.deltaTime;
                percentageCompleted = timeElapsed / duration;
                obj.transform.position = Vector3.Lerp(startingPos, endingPos.position, percentageCompleted);
                obj.transform.rotation = Quaternion.Lerp(startingRotation, endingRotation, percentageCompleted);
                yield return null;
            }

            obj.transform.position = endingPos.position;
            obj.transform.rotation = endingRotation;

            // Invoke the callback if provided
            onComplete?.Invoke();
        }


        public static IEnumerator ShakeObject(Rigidbody2D rb, float shakeDuration = 0.4f, float magnitude = 0.1f)
        {
            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                float xOffset = Random.Range(-1f, 1f) * magnitude;
                float yOffset = Random.Range(-1f, 1f) * magnitude;

                rb.MovePosition( rb.position + new Vector2(xOffset, yOffset));

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        public static IEnumerator ShakeObject(Transform objectToShake, float duration = 0.4f, float magnitude = 0.1f)
        {
            Vector3 originalPosition = objectToShake.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float xOffset = Random.Range(-1f, 1f) * magnitude;
                float yOffset = Random.Range(-1f, 1f) * magnitude;

                objectToShake.position = originalPosition + new Vector3(xOffset, yOffset, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            objectToShake.position = originalPosition;
        }

        public static void MoveObjectInRandomDirection(Transform obj, float magnitude = 1f)
        {
            float xAddition = Random.Range(-magnitude, magnitude);
            float yAddition = Random.Range(-magnitude, magnitude);
            obj.position += new Vector3(xAddition, yAddition, 0);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static IEnumerator WaitAndInvokeAction(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke();
        }

        public static IEnumerator ScaleObjectOverTime(GameObject objToScale, Vector3 targetScale, float duration)
        {
            Vector3 originalScale = objToScale.transform.localScale;

            // Scale up to the target scale
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                objToScale.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objToScale.transform.localScale = targetScale;
        }


        public static void StopAndStartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine,
            IEnumerator routine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }

            coroutine = monoBehaviour.StartCoroutine(routine);
        }


        public static bool CompareColors(Color color1, Color color2, int decimalPlaces = 2)
        {
            // Round each component to the specified number of decimal places
            float color1R = (float)Math.Round(color1.r, decimalPlaces);
            float color1G = (float)Math.Round(color1.g, decimalPlaces);
            float color1B = (float)Math.Round(color1.b, decimalPlaces);

            float color2R = (float)Math.Round(color2.r, decimalPlaces);
            float color2G = (float)Math.Round(color2.g, decimalPlaces);
            float color2B = (float)Math.Round(color2.b, decimalPlaces);

            // Compare rounded values directly
            bool redEqual = color1R == color2R;
            bool greenEqual = color1G == color2G;
            bool blueEqual = color1B == color2B;

            // Debug.Log($"Red Comparison: {color1R} vs {color2R} -> {redEqual}");
            // Debug.Log($"Green Comparison: {color1G} vs {color2G} -> {greenEqual}");
            // Debug.Log($"Blue Comparison: {color1B} vs {color2B} -> {blueEqual}");

            // Return true only if all channels are equal
            return redEqual && greenEqual && blueEqual;
        }

        public static IEnumerator ReduceFillRoutine(Image image, float initialFillAmount, float timeFrame)
        {
            float elapsedTime = 0f;
            float startFill = initialFillAmount;
            image.fillAmount = startFill;

            while (elapsedTime < timeFrame)
            {
                elapsedTime += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(startFill, 0, elapsedTime / timeFrame);
                yield return null;
            }

            image.fillAmount = 0;
        }

        public static TKey? GetRandomKey<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            // Extract keys as a list
            if (dictionary.Count == 0)
            {
                return default;
            }

            List<TKey> keys = new List<TKey>(dictionary.Keys);

            // Generate a random index
            int randomIndex = Random.Range(0, keys.Count);

            // Return the key at the random index
            return keys[randomIndex];
        }

        public static int CountPressedButtons()
        {
            int pressedButtonCount = 0;

            // List of valid keys for checking (filtered to prevent ArgumentOutOfRangeException)
            Key[] validKeys = new Key[]
            {
                Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M, Key.N, Key.O,
                Key.P, Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
                Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit6, Key.Digit5, Key.Digit7,
                Key.Digit8, Key.Digit9, // Top row numbers
                Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4, Key.Numpad5, Key.Numpad6, Key.Numpad7,
                Key.Numpad8, Key.Numpad9, // Numeric keypad
                Key.Space, Key.Enter, Key.Escape, Key.Backspace, Key.Tab, Key.CapsLock, Key.UpArrow, Key.DownArrow,
                Key.LeftArrow, Key.RightArrow
                // Add more keys as necessary
            };

            // Loop through valid keys
            foreach (Key key in validKeys)
            {
                if (Keyboard.current[key].isPressed)
                {
                    pressedButtonCount++;
                }
            }

            return pressedButtonCount;
        }


        public static IEnumerator LerpColor(Renderer renderer, Color fromColor, Color toColor, float duration,
            Action? onComplete = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the interpolation factor
                float t = elapsedTime / duration;

                // Lerp the color
                renderer.material.color = Color.Lerp(fromColor, toColor, t);

                // Increment elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Ensure the final color is set
            renderer.material.color = toColor;
            onComplete?.Invoke();
        }


        public static Vector3 UIToWorldNearCamera(RectTransform uiElement, Canvas canvas, UnityEngine.Camera camera,
            float distanceFromCamera = 5f)
        {
            Vector3 screenPoint = Vector3.zero;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Directly use the UI position since it's in screen space
                screenPoint = uiElement.position;
            }
            else
            {
                // Convert UI position to screen space
                screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiElement.position);
            }

            // Convert the screen position to a world point in front of the camera
            Vector3 worldPoint =
                camera.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, distanceFromCamera));

            return worldPoint;
        }


        public static IEnumerator RotateOverTime(Transform targetTransform, float degrees, float duration)
        {
            // Starting rotation
            float initialRotation = targetTransform.eulerAngles.z;
            float targetRotation = initialRotation + degrees;

            // Ensure target rotation stays within 0 to 360 range
            if (targetRotation > 360) targetRotation -= 360;
            else if (targetRotation < 0) targetRotation += 360;

            float elapsedTime = 0f;

            // Rotate gradually over time
            while (elapsedTime < duration)
            {
                float currentRotation = Mathf.LerpAngle(initialRotation, targetRotation, elapsedTime / duration);
                targetTransform.rotation = Quaternion.Euler(0, 0, currentRotation);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set to final rotation in case of precision errors
            targetTransform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
    }
}