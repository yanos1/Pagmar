using System;
using System.Collections.Generic;
using Interfaces;
using Player;
using Spine.Unity.Editor;
using UnityEngine;

namespace Enemies
{
    public class ArenaEnemySpawner : MonoBehaviour, IResettable
    {
        [SerializeField] private List<ChargingEnemy> enemies;
        private bool triggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {   
            if(triggered) return;
            if (other.GetComponent<PlayerManager>() is not null)
            {
                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            triggered = true;
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
            }
        }


        public void ResetToInitialState()
        {
            triggered = false;
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
            }
        }
    }
}
