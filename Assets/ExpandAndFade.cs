using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExpandAndFade : MonoBehaviour
{
    /* ── Exposed Settings ───────────────────────────────── */
    [Header("Timing")]
    [SerializeField] float lifeTime = 0.5f;
    [SerializeField] AnimationCurve sizeCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Size (local scale)")]
    [SerializeField] float finalScale = 2.5f;

    public enum FadeMode { Brightness, Alpha }
    [Header("Fade")]
    [SerializeField] FadeMode fadeMode = FadeMode.Brightness;

    /* ── Private state ─────────────────────────────────── */
    SpriteRenderer _sr;
    float  _elapsed;
    float  _startScale;
    bool   _playing;

    void Awake()
    {
        _sr         = GetComponent<SpriteRenderer>();
        _startScale = transform.localScale.x;          // take whatever scale you placed in Scene
    }

    /* Restart automatically whenever enabled (checkbox, SetActive, pooling, etc.) */
    void OnEnable()  => Restart();
    void OnDisable() => _playing = false;

    void Update()
    {
        if (!_playing) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / lifeTime);

        /* ── Scale ─────────────────────────────── */
        float size = Mathf.Lerp(_startScale, finalScale, sizeCurve.Evaluate(t));
        transform.localScale = new Vector3(size, size, 1f);

        /* ── Fade ──────────────────────────────── */
        switch (fadeMode)
        {
            case FadeMode.Brightness:
                // Stay fully opaque, dim RGB from white→black
                float bright = 1f - t;
                _sr.color = new Color(bright, bright, bright, 1f);
                break;

            case FadeMode.Alpha:
                // Traditional transparency fade (will pick up background hue)
                Color c = _sr.color;
                c.a = 1f - t;
                _sr.color = c;
                break;
        }

        /* ── Stop updating when done ───────────── */
        if (_elapsed >= lifeTime)
            _playing = false;
    }

    /* Manual restart (also called by OnEnable) */
    [ContextMenu("Restart Effect")]
    public void Restart()
    {
        _elapsed  = 0f;
        _playing  = true;
        _sr.color = Color.white;                    // full‑white, fully opaque
    }
}
