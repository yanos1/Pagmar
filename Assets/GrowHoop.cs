using UnityEngine;

/// <summary>
/// Expanding white ring (“hoop”) that
///   • grows from startRadius → finalRadius in lifeTime seconds,
///   • then destroys itself **and**
///   • spawns a particle‑effect prefab at the centre, handing it the hoop’s colour
///     and final radius for synchronised visuals.
/// Attach it to a GameObject that already has a LineRenderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class GrowHoop : MonoBehaviour
{
    /* ── Tweaks ─────────────────────────────────────────── */

    [Header("Timing")]
    [SerializeField] float lifeTime = 0.5f;
    [SerializeField] AnimationCurve sizeCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Radius (world units)")]
    [SerializeField] float startRadius = 0.2f;
    [SerializeField] float finalRadius = 2.5f;

    [Header("Visuals")]
    [SerializeField] int   segments   = 64;
    [SerializeField] float lineWidth  = 0.08f;
    [SerializeField] Color lineColor  = Color.white;

    [Header("Particles on Complete")]
    [Tooltip("Prefab with a ParticleSystem (optional).\n" +
             "If it has a script that implements IGrowHoopSync, " +
             "the hoop’s colour & radius are passed to it.")]
    [SerializeField] GameObject particlePrefab;

    /* ── Internals ──────────────────────────────────────── */

    LineRenderer _lr;
    float _elapsed;
    bool  _playing;

    /* ───────────────────────────────────────────────────── */

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();

        // Make sure the LineRenderer is ready
        _lr.loop          = true;
        _lr.useWorldSpace = false;
        _lr.positionCount = segments;
        _lr.startWidth    = _lr.endWidth = lineWidth;
        _lr.startColor    = _lr.endColor = lineColor;

        // Safe material: if no material set in the Inspector, give a basic one
        if (_lr.material == null)
            _lr.material = new Material(Shader.Find("Sprites/Default"));
    }

    /* Restart automatically whenever enabled */
    void OnEnable()  => Restart();
    void OnDisable() => _playing = false;

    void Update()
    {
        if (!_playing) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / lifeTime);

        float radius = Mathf.Lerp(startRadius, finalRadius, sizeCurve.Evaluate(t));
        DrawCircle(radius);

        if (_elapsed >= lifeTime)
        {
            _playing = false;
            SpawnParticles();   // 1️⃣ fire the VFX
            Destroy(gameObject); // 2️⃣ disappear
        }
    }

    /* Manual restart – also called by OnEnable */
    [ContextMenu("Restart Effect")]
    public void Restart()
    {
        _elapsed = 0f;
        _playing = true;
        DrawCircle(startRadius);
    }

    /* Draws the circle at the given radius */
    void DrawCircle(float r)
    {
        float step = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float a = Mathf.Deg2Rad * (i * step);
            _lr.SetPosition(i, new Vector3(Mathf.Cos(a) * r,
                                           Mathf.Sin(a) * r,
                                           0f));
        }
    }

    /* Spawns the particle effect and passes sync data if requested */
    void SpawnParticles()
    {
        if (particlePrefab == null) return;

        Instantiate(particlePrefab, transform.position, Quaternion.identity);
        // nothing else – the prefab stays exactly as saved in your Assets
    }

}

/* ── Optional interface for particle prefabs ────────────────────────── */
/* Implement this on a script inside your particle prefab if you want  */
/* to receive the hoop’s colour and final radius at spawn time.         */
public interface IGrowHoopSync
{
    void Init(Color hoopColor, float hoopRadius);
}
