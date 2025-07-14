using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Waits for a delay, fades one or more targets in, then triggers their Animators.
/// </summary>
public class MoveJoystich : MonoBehaviour
{
    [Header("Targets to fade‑in & animate")]
    [Tooltip("Add every GameObject you want to fade here (parent + children, images, etc.).")]
    [SerializeField] private List<GameObject> targets = new();   // drag >1 element here

    [Header("Timing")]
    [SerializeField] private float delayBeforeFade = 5f;
    [SerializeField] private float fadeDuration    = 1f;

    [Header("Animator")]
    [Tooltip("Trigger/state name sent to every Animator once fade completes. Leave blank to Play() the default state.")]
    [SerializeField] private string animatorTrigger = "Play";

    // ── internal data ─────────────────────────────────────
    struct FadeTarget
    {
        public CanvasGroup    cg;
        public SpriteRenderer sr;
        public Animator       anim;
    }
    FadeTarget[] _fts;

    // ── lifecycle ─────────────────────────────────────────
    void Awake()
    {
        if (targets.Count == 0)
        {
            Debug.LogError("[MoveJoystich] No targets assigned!", this);
            enabled = false;
            return;
        }

        // Cache components for speed
        _fts = new FadeTarget[targets.Count];
        for (int i = 0; i < targets.Count; i++)
        {
            GameObject go = targets[i];
            FadeTarget ft = new();

            // UI first
            ft.cg = go.GetComponent<CanvasGroup>();
            if (ft.cg == null && go.GetComponent<CanvasRenderer>() != null)
                ft.cg = go.AddComponent<CanvasGroup>();

            // World‑space sprite fallback
            ft.sr   = go.GetComponent<SpriteRenderer>();
            ft.anim = go.GetComponent<Animator>();

            SetAlpha(ft, 0f);          // start invisible
            _fts[i] = ft;
        }
    }

    void Start() => StartCoroutine(FadeInThenPlay());

    // ── coroutine ─────────────────────────────────────────
    IEnumerator FadeInThenPlay()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);

            foreach (var ft in _fts)
                SetAlpha(ft, a);

            yield return null;
        }

        // ensure final alpha = 1
        foreach (var ft in _fts)
            SetAlpha(ft, 1f);

        // trigger animators (optional)
        foreach (var ft in _fts)
        {
            if (ft.anim == null) continue;

            if (!string.IsNullOrEmpty(animatorTrigger))
                ft.anim.SetTrigger(animatorTrigger);
            else
                ft.anim.Play(0);   // play default state
        }
    }

    // ── helpers ───────────────────────────────────────────
    static void SetAlpha(FadeTarget ft, float a)
    {
        if (ft.cg) ft.cg.alpha = a;
        else if (ft.sr)
        {
            Color c = ft.sr.color;
            c.a = a;
            ft.sr.color = c;
        }
    }
}
