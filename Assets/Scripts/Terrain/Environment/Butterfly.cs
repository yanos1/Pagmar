using UnityEngine;

public class ButterflyMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 2f;

    [Header("Flutter Settings")]
    public float baseFlutterAmplitude = 0.4f;
    public float flutterAmplitudeVariance = 0.3f;
    public float flutterFrequency = 3f;
    private float flutterChangeInterval = 2f;
    public float flutterLerpSpeed = 1f;

    [Header("Noise Settings")]
    public float noiseStrengthX = 0.2f;
    public float noiseStrengthY = 0.2f;
    public float noiseTimeMultiplier = 0.2f; // Lower = smoother noise

    private float currentFlutterAmplitude;
    private float targetFlutterAmplitude;
    private float flutterChangeTimer;
    private float sineOffset;
    private float noiseOffsetX;
    private float noiseOffsetY;
    private float baseY;

    private bool canMove = false;

    void Start()
    {
        sineOffset = Random.Range(0f, Mathf.PI * 2);
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);

        baseY = transform.position.y;
        flutterChangeTimer = flutterChangeInterval;

        SetNewTargetFlutterAmplitude();
        currentFlutterAmplitude = targetFlutterAmplitude;
    }


    public void StartMoving()
    {
        canMove = true;
    }
    void Update()
    {
        if(!canMove) return;
        // Smoothly change flutter amplitude
        flutterChangeTimer -= Time.deltaTime;
        if (flutterChangeTimer <= 0f)
        {
            flutterChangeTimer = flutterChangeInterval + Random.Range(-1f,1f);
            SetNewTargetFlutterAmplitude();
        }
        currentFlutterAmplitude = Mathf.Lerp(currentFlutterAmplitude, targetFlutterAmplitude, flutterLerpSpeed * Time.deltaTime);

        // Smooth sine wave
        float flutter = Mathf.Sin(Time.time * flutterFrequency + sineOffset) * currentFlutterAmplitude;

        // Smoother noise using slower time
        float noiseTime = Time.time * noiseTimeMultiplier;
        float noiseX = (Mathf.PerlinNoise(noiseOffsetX, noiseTime) - 0.5f) * 2f * noiseStrengthX;
        float noiseY = (Mathf.PerlinNoise(noiseOffsetY, noiseTime) - 0.5f) * 2f * noiseStrengthY;

        Vector3 pos = transform.position;
        pos.x += (-forwardSpeed + noiseX) * Time.deltaTime;
        pos.y = baseY + flutter + noiseY;

        transform.position = pos;
    }

    void SetNewTargetFlutterAmplitude()
    {
        targetFlutterAmplitude = baseFlutterAmplitude + Random.Range(-flutterAmplitudeVariance, flutterAmplitudeVariance);
    }
}
