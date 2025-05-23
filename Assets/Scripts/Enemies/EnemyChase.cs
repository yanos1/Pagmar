using System;
using Managers;
using UnityEngine;

namespace Enemies
{
    public class EnemyChase : MonoBehaviour
    {

        [SerializeField] private GameObject navmeshSurface;
        [SerializeField] private ChasingEnemy _enemy;
        private void OnEnable()
        {
            CoreManager.Instance.EventManager.AddListener(EventNames.StartEnemyChase, OnStart);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.StartEnemyChase, OnStart);
        }

        private void OnStart(object obj)
        {
            print("start chase 76");
            navmeshSurface.SetActive(true);
            _enemy.StartChase();
        }
    }
}