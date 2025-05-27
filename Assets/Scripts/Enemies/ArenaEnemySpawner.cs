using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using Spine.Unity.Editor;
using Terrain.Environment;
using UnityEngine;

namespace Enemies
{
    public class ArenaEnemySpawner : MonoBehaviour, IResettable
    {
        [SerializeField] private List<ChargingEnemy> enemies;
        [SerializeField] private List<Gate> gates;
        private bool triggered = false;
        private void OnTriggerEnter2D(Collider2D other)
        {   
            if(triggered) return;
            if (other.GetComponent<PlayerManager>() is not null)
            {
                foreach (var gate in gates)
                {
                    gate.Open();
                }
                StartCoroutine(SpawnEnemies());
            }
        }

        private IEnumerator SpawnEnemies()
        {
            triggered = true;
            yield return new WaitForSeconds(2f);
            foreach (var enemy in enemies)
            {
                print("set enemy activate");
                enemy.gameObject.SetActive(true);
            }
        }


        public void ResetToInitialState()
        {
            triggered = false;
            foreach (var enemy in enemies)
            {
                enemy.ResetToInitialState();
                enemy.gameObject.SetActive(false);
            }
        }
    }
}
