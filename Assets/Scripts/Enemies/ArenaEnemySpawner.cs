using System.Collections;
using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
using Player;
using Terrain.Environment;
using UnityEngine;

namespace Enemies
{
    public class ArenaEnemySpawner : MonoBehaviour, IResettable
    {
        [SerializeField] private MMF_Player spawnFeedbacks;
        [SerializeField] private List<ChargingEnemy> enemies;
        [SerializeField] private List<Gate> gates;
        private bool triggered = false;


        private void Start()
        {
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {   
            if(triggered) return;
            if (other.GetComponent<PlayerManager>() is not null)
            {
                spawnFeedbacks?.PlayFeedbacks();
                StartCoroutine(SpawnCoroutine());
               ;
            }
        }

        private IEnumerator SpawnCoroutine()
        {
            yield return new WaitForSeconds(2f);
            foreach (var gate in gates)
            {
                gate.Open();
            }

            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            triggered = true;
            yield return new WaitForSeconds(0.5f);
            
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
