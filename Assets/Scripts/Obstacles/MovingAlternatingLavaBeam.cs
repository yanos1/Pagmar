using System;
using System.Collections;
using System.Collections.Generic;
using Atmosphere.TileExplostion;
using Enemies;
using FMODUnity;
using Interfaces;
using Managers;
using SpongeScene;
using Terrain.Environment;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Obstacles
{
    public class MovingAlternatingLavaBeam : AlternatingLavaBeam
    {
        public Tilemap tilemap;
        private Dictionary<Vector3Int, TileBase> removedTiles = new Dictionary<Vector3Int, TileBase>();

        [SerializeField] private float beamAdvanceDistance = 7f;
        [SerializeField] private int toggleLimit;
        [SerializeField] private ParticleSystem smokeParticleSystem;
        [SerializeField] private EventReference eruptionSound;
        [SerializeField] private EventReference buildUpSound;

        private Vector3 startingPos;
        private bool hasFinished;

        public override void Start()
        {
            base.Start();
            startingPos = transform.position;
        }
      
        public override void StartBeam()
        {
            this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        private IEnumerator ToggleBeam()
        {
            print("start beam");
            yield return new WaitForSeconds(delayBeforeFirstBeam);

            int toggleCount = 0;

            while (toggleCount < toggleLimit)
            {
                // Start feedback and warning
                startFeedbacks?.PlayFeedbacks();
                print("waiting beam..");
                yield return new WaitForSeconds(offTime - warningTime);

                // Build-up warning
                CoreManager.Instance.AudioManager.PlayOneShot(buildUpSound, transform.position);

                smokeParticleSystem.transform.position = new Vector3(
                    transform.position.x,
                    CoreManager.Instance.Player.transform.position.y
                );
                smokeParticleSystem.Play();

                yield return new WaitForSeconds(warningTime);

                // Beam ON
                col.enabled = true;
                print("BEAM ON!!");

                CoreManager.Instance.AudioManager.PlayOneShot(eruptionSound, transform.position);

                CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.LavaBurstParticles)
                    .Play(new Vector3(transform.position.x, -37, 0)); // lava is placed at -34 y

                yield return new WaitForSeconds(onTime);

                // Beam OFF
                col.enabled = false;
                print("resetin beam");

                if (onFinished is not EventNames.None)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onFinished, null);
                }

                // Move beam forward
                Vector3 futurePos = transform.position + Vector3.right * beamAdvanceDistance;
                transform.position = futurePos;
                print("move beam forward");

                toggleCount++;
            }

            // Beam is done
            hasFinished = true;
            col.enabled = false;
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IBreakable>() is { } breakable)
            {
                breakable.OnBreak();
            }
            CheckTileMapHit(other);
            
            // bad code
            if (other.gameObject.GetComponent<Explodable>() is { } explodable)
            {
                explodable.explode();
            }

            if (other.gameObject.GetComponent<ChargingEnemy>() is { } enemy)
            {
                enemy.Die();   
            }
        }

        private void CheckTileMapHit(Collider2D other)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);
            Vector3Int cellPos = tilemap.WorldToCell(hitPos);

            if (tilemap.HasTile(cellPos))
            {
                TileBase tile = tilemap.GetTile(cellPos);
                var particles = CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.ExplodableTileParticles);
                particles.Play(cellPos + Vector3.right*23.3f); // my grid is off by 23.3 ..... bad code.
                removedTiles.TryAdd(cellPos, tile); // Store only once
                tilemap.SetTile(cellPos, null);
            }
        }

        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            transform.position = startingPos;
            hasFinished = false;

            foreach (var kvp in removedTiles)
            {
                tilemap.SetTile(kvp.Key, kvp.Value);
            }

            removedTiles.Clear();
            smokeParticleSystem.Stop();
            startFeedbacks?.StopFeedbacks();
        }
    }
}
