using System.Collections;
using FMODUnity;
using Managers;
using UnityEngine;

namespace Atmosphere
{
    public class AirExplosion: MonoBehaviour
    {
        [SerializeField] private ExplosionForce f1;
        [SerializeField] private EventReference expSound;
        [SerializeField] private GameObject expPrefab;


        public void Explode()
        {
            StartCoroutine(Explodecoroutine());
            print("12 explode!");
        }

        private IEnumerator Explodecoroutine()
        {
            print("called exp");
            f1.doExplosion(CoreManager.Instance.Player.transform.position);
            yield break;
        }
    }
}