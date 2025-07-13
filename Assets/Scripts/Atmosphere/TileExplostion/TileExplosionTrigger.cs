using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Atmosphere.TileExplostion
{
    public class TileExplosionTrigger : MonoBehaviour, IResettable
    {
        [SerializeField] private SceneTrigger trigger;
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private ExplosionForce f;
        [SerializeField] private ExplodableTile t;
        [SerializeField] private MMF_Player explosionFeedbacks;
        [SerializeField] private MMF_Player resetFeedbacks;
        [SerializeField] private EventReference explosionSound;
        [SerializeField] private bool reset = true;
        private Dictionary<Vector3Int, TileBase> removedTiles = new Dictionary<Vector3Int, TileBase>();
        private Collider2D col;
        private bool exploded;
        private bool forceExplosionTriggered;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            col.enabled = false;
        }

        private void Update()
        {
            if (trigger.IsTriggered && !exploded)
            {
                exploded = true;
                StartExplosionSequence();
            }
        }

        private void StartExplosionSequence()
        {
            explosionFeedbacks?.PlayFeedbacks();
        }

        // we activate collider from inspector this couses the explosion
        private void OnTriggerStay2D(Collider2D other)
        {
            Vector3 hitPos = other.ClosestPoint(transform.position);
            Vector3Int cellPos = tilemap.WorldToCell(hitPos);

            if (tilemap.HasTile(cellPos))
            {
                TileBase tile = tilemap.GetTile(cellPos);
                if (removedTiles.TryAdd(cellPos, tile))
                {
                    tilemap.SetTile(cellPos, null);

                    CheckExplosionThreshold();
                }
                if (!forceExplosionTriggered)
                {
                    StartCoroutine(CreateExplosion(hitPos));
                }
            }
            
        }

        private IEnumerator CreateExplosion(Vector3 hitPos)
        {
            
            print("GROUIND EXPLODING!");
            if (!explosionSound.IsNull)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(explosionSound,transform.position);
            }
            forceExplosionTriggered = true;
            // ExplodableTile t = CoreManager.Instance.PoolManager.GetFromPool<ExplodableTile>(PoolEnum.ExplodableTile);
            t.gameObject.SetActive(true);
            t.Explode();
            f.doExplosion(f.transform.position);
            yield break;
            // yield return new WaitForSeconds(2f);
            // CoreManager.Instance.PoolManager.ReturnToPool(t);
        }

        private void CheckExplosionThreshold()
        {
            if (!forceExplosionTriggered && removedTiles.Count >= 10)
            {
                Debug.Log("Threshold reached. Triggering force explosion!");
            }
        }

        public void ResetToInitialState()
        {
            if(!reset) return;
            exploded = false;
            forceExplosionTriggered = false;
            col.enabled = false;
            resetFeedbacks?.PlayFeedbacks();

            foreach (var kvp in removedTiles)
            {
                tilemap.SetTile(kvp.Key, kvp.Value);
            }

            removedTiles.Clear();
        }
    }
}
