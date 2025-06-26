namespace UI
{
   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsDisplayer : MonoBehaviour
{
    [SerializeField] private List<GameObject> creditsObjects;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float visibleDuration = 3f;

    private int currentIndex = 0;

    public void DisplayCredits()
    {
        if (creditsObjects.Count > 0)
        {
            StartCoroutine(ShowCreditsLoop());
        }
    }

    private IEnumerator ShowCreditsLoop()
    {
        while (currentIndex < creditsObjects.Count)
        {
            GameObject current = creditsObjects[currentIndex];
            current.SetActive(true);

            var fades = GetFadableElements(current);
            yield return StartCoroutine(FadeElements(fades, 0f, 1f, fadeDuration)); // Fade in
            yield return new WaitForSeconds(visibleDuration); // Wait
            if (currentIndex == creditsObjects.Count - 1)
            {
                yield break;
            }
            yield return StartCoroutine(FadeElements(fades, 1f, 0f, fadeDuration)); // Fade out

            current.SetActive(false);
            currentIndex = currentIndex + 1;
        }
    }

    private List<Graphic> GetFadableElements(GameObject obj)
    {
        var graphics = new List<Graphic>();
        graphics.AddRange(obj.GetComponentsInChildren<Image>(true));
        graphics.AddRange(obj.GetComponentsInChildren<TextMeshProUGUI>(true));
        return graphics;
    }

    private IEnumerator FadeElements(List<Graphic> elements, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        foreach (var el in elements)
        {
            SetAlpha(el, startAlpha);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            foreach (var el in elements)
            {
                SetAlpha(el, alpha);
            }
            yield return null;
        }

        foreach (var el in elements)
        {
            SetAlpha(el, endAlpha);
        }
    }

    private void SetAlpha(Graphic g, float alpha)
    {
        if (g != null)
        {
            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
    }
}

}