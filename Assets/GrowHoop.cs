using UnityEngine;

/// <summary>
/// Expanding white ring (“hoop”) that restarts whenever the component/GameObject
/// is re‑enabled – handy for one‑click resets in the Inspector.
/// Attach it to a GameObject that already has a LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class GrowHoop : MonoBehaviour
{
    /* ── Tweaks ─────────────────────────────────────────── */

    [Header("Timing")]
    [SerializeField] float lifeTime = 0.5f;         // seconds to finish
    [SerializeField] AnimationCurve sizeCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);       // ease‑out

    [Header("Radius")]
    [SerializeField] float startRadius = 0.2f;      // world units
    [SerializeField] float finalRadius = 2.5f;

    [Header("Visuals")]
    [SerializeField] int   segments   = 64;         // circle fidelity
    [SerializeField] float lineWidth  = 0.08f;

    /* ── Internals ──────────────────────────────────────── */

    LineRenderer _lr;
    float _elapsed;
    bool  _playing;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();

        // Set some sensible defaults for the line
        _lr.loop        = true;
        _lr.useWorldSpace = false;                  // local space so parent scaling works
        _lr.positionCount = segments;
        _lr.startColor = _lr.endColor = Color.white;
        _lr.startWidth = _lr.endWidth = lineWidth;
    }

    /* Restart whenever enabled (toggle checkbox, SetActive, etc.) */
    void OnEnable()  => Restart();
    void OnDisable() => _playing = false;

    void Update()
    {
        if (!_playing) return;

        _elapsed += Time.deltaTime;
        float t   = Mathf.Clamp01(_elapsed / lifeTime);

        float radius = Mathf.Lerp(startRadius, finalRadius, sizeCurve.Evaluate(t));
        DrawCircle(radius);

        if (_elapsed >= lifeTime)
            _playing = false;
    }

    /* Manual restart – also called by OnEnable */
    [ContextMenu("Restart Effect")]
    public void Restart()
    {
        _elapsed = 0f;
        _playing = true;
        DrawCircle(startRadius);                    // start small
    }

    /* Draws the circle at the given radius */
    void DrawCircle(float r)
    {
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * angleStep);
            _lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * r,
                                           Mathf.Sin(angle) * r,
                                           0f));
        }
    }
}
