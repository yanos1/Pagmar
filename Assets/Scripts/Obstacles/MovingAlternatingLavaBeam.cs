using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Managers;
using SpongeScene;
using Terrain.Environment;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Obstacles
{
    public class MovingAlternatingLavaBeam : AlternatingLavaBeam
    {
        public Tilemap tilemap;
        private Dictionary<Vector3Int, TileBase> removedTiles = new Dictionary<Vector3Int, TileBase>();

        [SerializeField] private float beamAdvanceDistance = 7f;
        [SerializeField] private int toggleLimit = 12;

        private Vector3 startingPos;
        private bool hasFinished;

        public override void StartBeam()
        {
            startingPos = transform.position;
            this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        private IEnumerator ToggleBeam()
        {
            yield return new WaitForSeconds(delayBeforeFirstBeam);

            int toggleCount = 0;

            while (toggleCount < toggleLimit)
            {
                // Start feedback and warning
                startFeedbacks?.PlayFeedbacks();
                yield return new WaitForSeconds(offTime - warningTime);

                warning.transform.position = new Vector3(
                    transform.position.x,
                    CoreManager.Instance.Player.transform.position.y + 2.3f,
                    0
                );
                StartCoroutine(UtilityFunctions.FadeImage(warning, 0.6f, 0, warningTime, null));
                yield return new WaitForSeconds(warningTime);

                // Beam ON
                beamSprite.enabled = true;
                col.enabled = true;
                yield return new WaitForSeconds(onTime);

                // Beam OFF
                beamSprite.enabled = false;
                col.enabled = false;

                // Trigger event if needed
                if (onFinished is not EventNames.None)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onFinished, null);
                }

                // Move beam forward
                Vector3 futurePos = transform.position + Vector3.right * beamAdvanceDistance;
                transform.position = futurePos;

                // Update warning position for next cycle
                warning.transform.position = new Vector3(
                    futurePos.x,
                    CoreManager.Instance.Player.transform.position.y + 2.3f,
                    0
                );

                toggleCount++;
            }

            // Beam is done
            hasFinished = true;
            beamSprite.enabled = false;
            col.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IBreakable>() is { } breakable)
            {
                breakable.OnBreak();
            }
            print($"hit {other.gameObject.name}");
            CheckTileMapHit(other);
        }

        private void CheckTileMapHit(Collider2D other)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);
            Vector3Int cellPos = tilemap.WorldToCell(hitPos);
            Debug.Log($"Cell position hit: {cellPos}");

            if (tilemap.HasTile(cellPos))
            {
                TileBase tile = tilemap.GetTile(cellPos);
                removedTiles.TryAdd(cellPos, tile); // Store only once
                tilemap.SetTile(cellPos, null);
                Debug.Log($"Removed tile at {cellPos}");
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
                Debug.Log("Restored tile at: " + kvp.Key);
            }

            removedTiles.Clear();
        }
    }
}
