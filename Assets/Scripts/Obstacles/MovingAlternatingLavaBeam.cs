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


        [SerializeField] private float targetXPosition;
        private bool hasReachedTarget;
        private Vector3 startingPos;

        public override void StartBeam()
        {
            startingPos = transform.position;
            this.StopAndStartCoroutine(ref toggleCoroutine, ToggleBeam());
        }

        private IEnumerator ToggleBeam()
        {
            yield return new WaitForSeconds(delayBeforeFirstBeam);

            while (!hasReachedTarget)
            {
                if (isOff)
                {
                    startFeedbacks?.PlayFeedbacks();
                    yield return new WaitForSeconds(offTime - warningTime);
                    warning.transform.position = new Vector3(transform.position.x, CoreManager.Instance.Player.transform.position.y + 2.3f, 0);
                    StartCoroutine(UtilityFunctions.FadeImage(warning, 0.6f, 0, warningTime, null));
                    yield return new WaitForSeconds(warningTime);
                }
                else
                {
                    yield return new WaitForSeconds(onTime);
                }

                isOff = !isOff;

                // Only turn ON if still within target
                if (!isOff && transform.position.x >= targetXPosition)
                {
                    hasReachedTarget = true;
                    beamSprite.enabled = false;
                    col.enabled = false;
                    yield break;
                }

                beamSprite.enabled = !isOff;
                col.enabled = !isOff;

                if (onFinished is not EventNames.None && col.enabled == false)
                {
                    CoreManager.Instance.EventManager.InvokeEvent(onFinished, null);
                }

                float beamAdvanceDistance = 4.3f;
                Vector3 futurePos = transform.position + Vector3.right * beamAdvanceDistance;
                warning.transform.position =
                    new Vector3(futurePos.x, CoreManager.Instance.Player.transform.position.y + 2.3f, 0);
                transform.position = futurePos;
            }
        }

      

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<IBreakable>() is { } breakable)
            {
                breakable.OnBreak();
            }

            CheckTileMapHit(other);
        }

        private void CheckTileMapHit(Collider2D other)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);
            Vector3Int cellPos = tilemap.WorldToCell(hitPos);
            print($"cell pose hit {cellPos}");

            if (tilemap.HasTile(cellPos))
            {            
                print($"found tile to remove! {cellPos}");

                TileBase tile = tilemap.GetTile(cellPos);
                removedTiles.TryAdd(cellPos, tile); // Only store once
                tilemap.SetTile(cellPos, null);  // Remove tile
                print("removed tile!");
            }
        }
        
        public override void ResetToInitialState()
        {
            base.ResetToInitialState();
            transform.position = startingPos;
            hasReachedTarget = false;
            foreach (var kvp in removedTiles)
            {
                tilemap.SetTile(kvp.Key, kvp.Value);  // Restore tile
                Debug.Log("Restored tile at: " + kvp.Key);
            }

            removedTiles.Clear();  // Optional: clear the list after resetting
        }
    }
}