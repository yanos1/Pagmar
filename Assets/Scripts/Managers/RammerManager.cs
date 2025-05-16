using System;
using Interfaces;
using UnityEngine.Serialization;

namespace Managers
{
    using UnityEngine;

    public class RammerManager : MonoBehaviour
    {
        public static RammerManager Instance;

        [SerializeField] private float baseForce;

        private void Awake()
        {
            Instance = this;
        }

        public void ResolveRam(Rammer a, Rammer b)
        {
            float forceA = a.CurrentForce;
            float forceB = b.CurrentForce;
            print($"{a.CurrentForce} {b.CurrentForce}");
            // If same force, both knockback
            if (Mathf.Approximately(forceA, forceB))
            {
                print("no winner no loser! 55");
                Vector2 dirA = (a.transform.position - b.transform.position).normalized;
                dirA = new Vector2(dirA.x, 0.5f);
                Vector2 dirB = new Vector2(-dirA.x, 0.5f);

                a.ApplyKnockback(dirA, baseForce);
                b.ApplyKnockback(dirB, baseForce);

                return;
            }

            // Determine winner and loser
            Rammer winner = forceA > forceB ? a : b;
            Rammer loser = forceA > forceB ? b : a;
            print($"rammer is {winner} 55 ");
            print($"rammed is {loser} 55");
            float winnerForce = Mathf.Max(forceA, forceB);
            float loserForce = Mathf.Min(forceA, forceB);

            // Winner rams, loser gets rammed
            winner.OnRam(loserForce);
            loser.OnRammed(winnerForce);
            
            // Knockback loser
            Vector2 direction = (loser.transform.position - winner.transform.position).normalized;
            loser.ApplyKnockback(new Vector2(direction.x, 0.5f), (winnerForce-loserForce) * baseForce);
        }
    }

}