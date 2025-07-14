using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteFader : MonoBehaviour
{
    [Header("Sprite Targets")]
    [SerializeField] private List<SpriteRenderer> targets = new List<SpriteRenderer>();
    [SerializeField] private Animator animator; 
    [Header("Timing (seconds)")]
    [SerializeField] private float delay = 1f;
    [SerializeField] private float fadeDuration = 1f;

    private float savedAlpha = 1f;
    private Coroutine activeFade;

    private void Awake()
    {
        // Auto-fill with attached SpriteRenderers if none are assigned
        if (targets.Count == 0)
        {
            SpriteRenderer[] found = GetComponentsInChildren<SpriteRenderer>();
            targets.AddRange(found);
        }

        if (targets.Count > 0)
            savedAlpha = targets[0].color.a;

        SetAlpha(0f); // start fully transparent
    }

    private void OnEnable()
    {
        StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        yield return FadeTo(savedAlpha);
    }

    public void TriggerFadeOut()
    {
        if (activeFade == null)
            activeFade = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (animator != null)
        {
            animator.enabled = !animator.enabled;
        }
        float startAlpha = targets.Count > 0 ? targets[0].color.a : 0f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerpAlpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            SetAlpha(lerpAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
        activeFade = null;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var sr in targets)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    { 
        TriggerFadeOut();
    }
}