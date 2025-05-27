using Interfaces;
using Triggers;
using UnityEngine;

public class MovingFire : MonoBehaviour, IKillPlayer, IResettable
{
    [SerializeField] private Trigger _trigger;

    [Header("Beam Movement")] public Transform leftPoint;
    public Transform rightPoint;
    public float moveSpeed = 2f;

    private bool movingLeft = true;

    [Header("Fade Settings")] public float fadeDuration = 0.1f;
    public float fadeSizeMultiplier = 0.3f;
    public float fadeAlpha = 0.2f;

    [Header("Debug")] public bool debugPrint = false;

    private ParticleSystem fireParticleSystem;
    private ParticleSystem.Particle[] particles;
    private float initialAlpha = 1f;
    private Vector3 startPos;

    void Start()
    {
        transform.position = rightPoint.position;
        startPos = transform.position;
        fireParticleSystem = GetComponentInChildren<ParticleSystem>();
        if (fireParticleSystem == null)
        {
            Debug.LogError("FireBeam: No ParticleSystem found!");
            return;
        }

        // Store the initial alpha from the main module
        initialAlpha = fireParticleSystem.main.startColor.color.a;
    }

    void Update()
    {
        if (_trigger.IsTriggered)
        {
            MoveBeam();
        }
    }

    private void MoveBeam()
    {
        Transform target = movingLeft ? leftPoint : rightPoint;
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            movingLeft = !movingLeft;
        }
    }

    public bool IsDeadly()
    {
        return true;
    }

    public void ResetToInitialState()
    {
        transform.position = startPos;
        
    }
}