using System;
using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Atmosphere.TileExplostion;
using Enemies;
using FMOD.Studio;
using FMODUnity;
using Managers;
using Spine.Unity;
using UnityEngine.Tilemaps;
using EventManager = FMODUnity.EventManager;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class ChasingEnemy : Rammer, IResettable
{
    [SerializeField] private Rammer player;
    [SerializeField] private EventReference crawlSound;
    private EventInstance crawlInstance;
    [SerializeField] private EventReference biteSound;
    [SerializeField] private List<EventReference> roarSounds;
    [SerializeField] private SkeletonAnimation skeletonAnimation;


    private float contactRadius;
    private NavMeshAgent agent;
    private float rotationSpeed = 3f;
    private Collider2D col;
    private Vector3 startingPosition;
    private Vector3 playerResetPosition;
    private bool chase = false;
    private float positionHistoryDuration = 17f;
    private List<(float time, Vector3 position)> positionHistory = new List<(float, Vector3)>();

    public Tilemap tilemap;
    private Dictionary<Vector3Int, TileBase> removedTiles = new Dictionary<Vector3Int, TileBase>();

    // Audio state
    private bool crawlSoundPlaying = false;
    private bool bitePlaying = false;
    private bool roarPlaying = false;
    private bool hasRoaredFirst = false;
    private float biteCooldown = 4.5f;
    private float roarCooldown = 5.2f;
    private float biteSoundTimer = 4.5f;
    private float roarSoundTimer = 5.2f;

    // Caching
    private Vector3 lastProcessedPlayerResetPos = Vector3.positiveInfinity;
    private Vector3 cachedBestResetPosition = Vector3.zero;
    private bool hasCachedReset = false;

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.ReachedCheckPoint, OnReachedCheckpoint);
        StopAllCoroutines();
        crawlInstance.stop(STOP_MODE.IMMEDIATE);
        crawlInstance.release();
    }

    private void OnReachedCheckpoint(object obj)
    {
        if (obj is Vector3 pos)
        {
            playerResetPosition = pos;
        }
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<Collider2D>();
        contactRadius = Mathf.Abs(col.bounds.max.x - transform.position.x);
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        CurrentForce = 5;
        startingPosition = transform.position;
    }

    void Update()
    {
        if (!chase)
            return;

        if (!crawlSoundPlaying)
        {
            crawlSoundPlaying = true;
            crawlInstance = CoreManager.Instance.AudioManager.CreateEventInstance(crawlSound);
            crawlInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            crawlInstance.start();
            print("try crawl sound");
            skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);
        }

        // Timers
        if (biteSoundTimer > 0)
            biteSoundTimer -= Time.deltaTime;

        if (roarSoundTimer > 0)
            roarSoundTimer -= Time.deltaTime;

        // Bite logic
        if (biteSoundTimer <= 0 && !bitePlaying)
        {
            StartCoroutine(PlayBiteSound());
            biteSoundTimer = biteCooldown;
        }

        // Roar logic
        if (roarSoundTimer <= 0 && !roarPlaying && !bitePlaying)
        {
            StartCoroutine(PlayRoarSound());
            roarSoundTimer = roarCooldown;
        }

        agent.SetDestination(player.transform.position);

        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
        float distance = Vector2.Distance(enemyPos, playerPos);

        if (distance <= contactRadius)
        {
            // RammerManager.Instance.ResolveRam(player, this); // handled by player
        }

        Vector3 velocity = agent.velocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            Vector3 direction = velocity.normalized;
            float targetAngle = MapYToZRotation(direction.y);
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private IEnumerator PlayBiteSound()
    {
        bitePlaying = true;
        CoreManager.Instance.AudioManager.PlayOneShot(biteSound, transform.position);
        yield return new WaitForSeconds(0.5f); // bite sound duration
        bitePlaying = false;

        // If roar was due and waiting, play immediately
        if (roarSoundTimer <= 0 && !roarPlaying)
        {
            StartCoroutine(PlayRoarSound());
            roarSoundTimer = roarCooldown;
        }
    }

    private IEnumerator PlayRoarSound()
    {
        roarPlaying = true;

        EventReference selectedRoar;
        if (!hasRoaredFirst)
        {
            selectedRoar = roarSounds[0];
            hasRoaredFirst = true;
        }
        else
        {
            selectedRoar = roarSounds[Random.Range(0, roarSounds.Count)];
        }

        CoreManager.Instance.AudioManager.PlayOneShot(selectedRoar, transform.position);
        yield return null; // add delay here if you want to prevent overlap again
        roarPlaying = false;
    }

    private void CheckTileMapHit(Collider2D other)
    {
        Vector3 hitPos = other.bounds.center;
        Vector3Int centerCell = tilemap.WorldToCell(hitPos);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (Random.value < 0.75f) continue;
                Vector3Int offset = new Vector3Int(x, y, 0);
                Vector3Int targetCell = centerCell + offset;

                if (tilemap.HasTile(targetCell))
                {
                    TileBase tile = tilemap.GetTile(targetCell);

                    if (removedTiles.TryAdd(targetCell, tile))
                    {
                        tilemap.SetTile(targetCell, null);
                        Vector3 worldPos = tilemap.GetCellCenterWorld(targetCell);
                        var particles = CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.ExplodableTileParticles);
                        particles.Play(worldPos);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<IBreakable>() is { } breakable)
        {
            breakable.OnBreak();
        }

        CheckTileMapHit(other);
    }

    private float MapYToZRotation(float y)
    {
        return Mathf.Lerp(0f, 180f, (y + 1f) / 2f);
    }

    public override void OnRam(Vector2 ramDirNegative, float againstForce)
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public override void OnRammed(float fromForce, Vector3 collisionPoint) { }

    public override void OnTie(float fromForce, Vector3 collisionPoint) { }

    public override void ApplyKnockback(Vector2 direction, float force) { }

    public void ResetToInitialState()
    {
        if (!chase) return;

        foreach (var kvp in removedTiles)
        {
            tilemap.SetTile(kvp.Key, kvp.Value);
        }

        removedTiles.Clear();

        if (hasCachedReset && playerResetPosition == lastProcessedPlayerResetPos)
        {
            agent.Warp(cachedBestResetPosition);
            agent.isStopped = false;
            print($"[CACHED] chosen pos {cachedBestResetPosition}");
            return;
        }

        var considerablePositions = new Dictionary<float, Vector3>();
        foreach (var (time, pos) in positionHistory)
        {
            float distance = Vector3.Distance(playerResetPosition, pos);
            if (pos.x < playerResetPosition.x && distance > 14.5f)
            {
                considerablePositions[distance] = pos;
            }
        }

        if (considerablePositions.Count > 0)
        {
            float closestDistance = -1f;
            float smallestDiff = float.MaxValue;

            foreach (var kvp in considerablePositions)
            {
                float diff = Mathf.Abs(kvp.Key - 18f);
                if (diff < smallestDiff)
                {
                    smallestDiff = diff;
                    closestDistance = kvp.Key;
                }
            }

            Vector3 bestPosition = considerablePositions[closestDistance];
            agent.Warp(bestPosition);
            agent.isStopped = false;
            print($"[CALCULATED] chosen pos {bestPosition}");

            lastProcessedPlayerResetPos = playerResetPosition;
            cachedBestResetPosition = bestPosition;
            hasCachedReset = true;
        }
    }

    public void StartChase()
    {
        chase = true;
        StartCoroutine(RecordPositionHistory());
    }

    private IEnumerator RecordPositionHistory()
    {
        while (chase)
        {
            positionHistory.Add((Time.time, transform.position));
            while (positionHistory.Count > 0 && Time.time - positionHistory[0].time > positionHistoryDuration)
            {
                positionHistory.RemoveAt(0);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
