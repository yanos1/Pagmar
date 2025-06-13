using System.Collections;
using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Utility.תונוש
{
    public class TransformationComics : MonoBehaviour, IResettable
    {
        [SerializeField] private List<GameObject> ex;
        [SerializeField] private ExplosionForce f; // force after we exploded each
        public void StartComics()
        {
            GetComponent<MMF_Player>()?.PlayFeedbacks();
        }
        public void Explode()
        {
            foreach (var e in ex)
            {
                
                // e.GetComponent<Explodable>().explode();
                // e.GetComponent<ExplosionForce>().doExplosion(e.transform.position);
            }

            StartCoroutine(DoSeconderyExplosionAdfterDelay());
        }

        private IEnumerator DoSeconderyExplosionAdfterDelay()
        {
            yield return new WaitForSecondsRealtime(1.2f);
            print("explodede secondery!!!!");
            foreach (var e in ex)
            {
                
                e.GetComponent<Explodable>().explode();
                e.GetComponentInChildren<ExplosionForce>().doExplosion(e.transform.position);
            }
        }

        public void SecondaryExplosuion()
        {
            f.doExplosion(f.transform.position);
        }

        public void ResetToInitialState()
        {
            foreach (var e in ex)
            {
                e.SetActive(false);
            }
        }
    }
}