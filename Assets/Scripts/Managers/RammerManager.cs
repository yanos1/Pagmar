using System;
using Interfaces;
using UnityEngine.Serialization;

namespace Managers
{
    using UnityEngine;

    public class RammerManager : MonoBehaviour
    {
        public static RammerManager Instance;
        
        [SerializeField] private float baseForce = 30f;

        private void Awake()
        {
            Instance = this;
        }

        public void ResolveRam(Rammer a, Rammer b)
        {
            float forceA = a.CurrentForce;
            float forceB = b.CurrentForce;

            // If same force, both knockback
            if (Mathf.Approximately(forceA, forceB))
            {
                Vector2 dirA = (a.transform.position - b.transform.position).normalized;
                dirA = new Vector2(dirA.x, 0);
                Vector2 dirB = -dirA;

                a.ApplyKnockback(dirA, forceA * baseForce);
                b.ApplyKnockback(dirB, forceB * baseForce);

                return;
            }

            // Determine winner and loser
            Rammer winner = forceA > forceB ? a : b;
            Rammer loser = forceA > forceB ? b : a;
            print($"rammer is {winner} 55 ");
            print($"rammed is {winner} 55");
            float winnerForce = Mathf.Max(forceA, forceB);
            float loserForce = Mathf.Min(forceA, forceB);

            // Winner rams, loser gets rammed
            winner.OnRam(loserForce);
            loser.OnRammed(winnerForce);
            
            // Knockback loser
            Vector2 direction = (loser.transform.position - winner.transform.position).normalized;
            loser.ApplyKnockback(new Vector2(direction.x, 0), (winnerForce-loserForce) * baseForce);
        }
    }

}