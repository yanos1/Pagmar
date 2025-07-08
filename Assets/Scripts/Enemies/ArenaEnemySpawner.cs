using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Managers;
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
        private int timesActivated = 0;
        private bool finishedBattle = false;


        private void Start()
        {
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggered) return;
            if (other.GetComponent<PlayerManager>() is { } player)
            {
                spawnFeedbacks?.PlayFeedbacks();
                StartCoroutine(SpawnCoroutine());
            }
        }

        public void AddKnockBacktoEnemies()
        {
            foreach (var enemy in enemies)
            {
                enemy.AffectedByExternalKnockback();
            }
        }

        private IEnumerator SpawnCoroutine()
        {
            yield return new WaitForSeconds(2f);
            foreach (var gate in gates)
            {
                gate.Open();
            }

            timesActivated++;
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            triggered = true;
            yield return new WaitForSeconds(0.3f);
            int countTwo = 0;
            foreach (var enemy in enemies)
            {
                print("set enemy activate");
                enemy.gameObject.SetActive(true);
                if (++countTwo % 2 == 0)
                {
                    yield return new WaitForSeconds(0.25f);
                }
            }

            StartCoroutine(OpenGatesWhenAllDead());
        }

        private IEnumerator OpenGatesWhenAllDead()
        {
            while (!finishedBattle)
            {
                yield return new WaitForSeconds(2f);
                if (enemies.TrueForAll(enemy =>
                        !enemy.gameObject.activeInHierarchy))
                {
                    finishedBattle = true;
                    OpenGates(); 
                
                    yield break; 
                }
            }
        }

        private void OpenGates()
        {
            foreach (var gate in gates)
            {
                gate.OpenDontClose();
            }
            CoreManager.Instance.Player.ExitGodMode();
            CoreManager.Instance.AudioManager.SetGlobalParameter("ColloseumEnemiesCleared", 1);
        }
        public void ResetToInitialState()
        {
            StopAllCoroutines();
            triggered = false;
            foreach (var enemy in enemies)
            {
                enemy.ResetToInitialState();
                enemy.gameObject.SetActive(false);
            }

            if (timesActivated > 0 && !finishedBattle)  // this means the player has diied and is trying again to do the arena battle. we wait the cameras here to show enemies then give input back
            {
                CoreManager.Instance.Player.DisableInputForDuration(5f);
            }
        }
    }
}