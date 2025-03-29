using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using SpongeScene;
using UnityEngine;

namespace Obstacles.Shooters
{
    public class ShooterController : MonoBehaviour
    {
        [SerializeReference] private List<Shooter> shooters;

        [SerializeField] private float delayBetweenShots;
        [SerializeField] private float waves;
        [SerializeField] private float warningDuration = 2;

        [SerializeField] private SpriteRenderer warningRenderer;
        [SerializeField] private GameObject warning;

        private bool cantShoot = false;
        private int timesActivated = 0;

        private IEnumerator Shoot()
        {
            for (int i = 0; i < waves; ++i)
            {
                warning.SetActive(true);
                yield return StartCoroutine(UtilityFunctions.FadeImage(warningRenderer, 0.6f, 0,
                    warningDuration));
                
                foreach (var shooter in shooters)
                {
                    shooter.Shoot();
                }
                yield return new WaitForSeconds(delayBetweenShots);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerMovement>() is not null || other.GetComponent<PlayerMovement2>() is not null)
            {
                if (cantShoot) return;
                StartCoroutine(Shoot());
                StartCoroutine(CantShootForDuration());
            }

        }

        private IEnumerator CantShootForDuration() // this is BAD CODE - fix later
        {
            cantShoot = true;
            if(++timesActivated == 2 ) yield break; 
            yield return new WaitForSeconds(2.5f);
            cantShoot = false;
        }
    }
}