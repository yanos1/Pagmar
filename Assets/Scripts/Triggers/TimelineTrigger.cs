using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                 // for UI Graphic base class

/// <summary>
/// After <delayBeforeFade> seconds, fades ALL SpriteRenderers & UI Graphics
/// under <target> from α 0 → 1 in <fadeDuration> seconds, then triggers an
/// Animator trigger on that hierarchy (optional).
///
/// Attach this to any convenient GameObject (for example, the joystick root).
/// </summary>
public class MoveJoystick : MonoBehaviour
{
    [Header("Target root to fade‑in & animate")]
    [SerializeField] private GameObject target;

    [Header("Timing")]
    [SerializeField] private float delayBeforeFade = 5f;
    [SerializeField] private float fadeDuration    = 1f;

    [Header("Animator")]
    [Tooltip("Trigger name to fire when fade completes. Leave blank to skip.")]
    [SerializeField] private string animatorTrigger = "Play";

    /* ── cached lists ────────────────────────────────────── */
    private readonly List<SpriteRenderer> _sprites  = new();
    private readonly List<Graphic>        _graphics = new();   // UI Images / Text & TMP
    private Animator _anim;

    /* ----------------------------------------------------- */
    private void Awake()
    {
        if (target == null)
        {
            Debug.LogError("[MoveJoystick] No target assigned!", this);
            enabled = false;
            return;
        }

        // Collect renderers (includes INACTIVE children)
        _sprites .AddRange(target.GetComponentsInChildren<SpriteRenderer>(true));
        _graphics.AddRange(target.GetComponentsInChildren<Graphic>(true));

        // Optional animator (could be anywhere under the target)
        _anim = target.GetComponentInChildren<Animator>(true);

        // Force everything invisible before the first frame
        SetAlphaAll(0f);
    }

    private void Start()
    {
        StartCoroutine(FadeInThenPlay());
    }

    /* ----------------------------------------------------- */
    private IEnumerator FadeInThenPlay()
    {
        // 1) Wait
        yield return new WaitForSeconds(delayBeforeFade);

        // 2) Fade 0 → 1
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            SetAlphaAll(a);
            yield return null;
        }
        SetAlphaAll(1f);            // ensure full opacity

        // 3) Trigger the animation, if requested
        if (_anim != null && !string.IsNullOrEmpty(animatorTrigger))
            _anim.SetTrigger(animatorTrigger);
    }

    /* Helper: set alpha on every renderer we cached */
    private void SetAlphaAll(float a)
    {
        foreach (var sr in _sprites)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }

        foreach (var g in _graphics)
        {
            if (g == null) continue;
            Color c = g.color;
            c.a = a;
            g.color = c;
        }
    }
}
