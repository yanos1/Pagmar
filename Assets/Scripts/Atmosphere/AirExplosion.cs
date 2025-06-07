using System.Collections;
using UnityEngine;

namespace Atmosphere
{
    public class AirExplosion: MonoBehaviour
    {
        [SerializeField] private ExplosionForce f1;


        public void Explode()
        {
            StartCoroutine(Explodecoroutine());
            print("12 explode!");
        }

        private IEnumerator Explodecoroutine()
        {
            // yield return new WaitForSeconds(0f);
            f1.doExplosion(transform.position);
            yield break;
        }
    }
}