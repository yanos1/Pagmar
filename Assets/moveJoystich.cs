using System.Collections;
using UnityEngine;

/// <summary>
/// Waits for a delay, fades a target in, then triggers its Animator.
/// Attach this to any GameObject (for example the joystick root).
/// </summary>
public class MoveJoystich : MonoBehaviour
{
    [Header("Target to fade‑in & animate")]
    [SerializeField] private GameObject target;

    [Header("Timing")]
    [SerializeField] private float delayBeforeFade = 5f;   // seconds to wait
    [SerializeField] private float fadeDuration    = 1f;   // seconds to fade from 0 → 1 alpha

    [Header("Animator")]
    [Tooltip("Name of the trigger or state to play when fade finishes.")]
    [SerializeField] private string animatorTrigger = "Play";

    // ── private refs ──────────────────────────────────────
    CanvasGroup   _cg;
    SpriteRenderer _sr;
    Animator       _anim;

    void Awake()
    {
        if (target == null)
        {
            Debug.LogError("[MoveJoystick] No target assigned!", this);
            enabled = false;
            return;
        }

        // Try CanvasGroup first (best for UI)
        _cg = target.GetComponent<CanvasGroup>();
        if (_cg == null && target.GetComponent<CanvasRenderer>() != null)
            _cg = target.AddComponent<CanvasGroup>();   // add one so we can fade UI Images/Text

        // World‑space sprite fallback
        _sr = target.GetComponent<SpriteRenderer>();

        // Animator (optional)
        _anim = target.GetComponent<Animator>();

        // Start invisible
        SetAlpha(0f);
    }

    void Start()
    {
        StartCoroutine(FadeInThenPlay());
    }

    IEnumerator FadeInThenPlay()
    {
        // 1) wait
        yield return new WaitForSeconds(delayBeforeFade);

        // 2) fade 0 → 1
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(1f);

        // 3) animate
        if (_anim != null)
        {
            if (!string.IsNullOrEmpty(animatorTrigger))
                _anim.SetTrigger(animatorTrigger);
            else
                _anim.Play(0); // play default state
        }
    }

    /* Helper: sets alpha on whichever component we have */
    void SetAlpha(float a)
    {
        if (_cg) _cg.alpha = a;
        else if (_sr)
        {
            Color c = _sr.color;
            c.a = a;
            _sr.color = c;
        }
    }
}
