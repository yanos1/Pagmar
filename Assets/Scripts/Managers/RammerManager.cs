using System;
using Interfaces;
using Player;
using UnityEngine.Rendering;
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
            
            Vector2 dirA = (a.transform.position - b.transform.position).normalized;
            if (forceA ==0 && forceB == 0 && a.IsCharging) 
            {
                a.ApplyKnockback(dirA, baseForce/2);

                return;
            }
            dirA = new Vector2(dirA.x, 0.5f);
            
            // If same force, both knockback
            if (Mathf.Approximately(forceA, forceB))
            {
                if (forceA > 0)
                {
                    print("no winner no loser! 55");
               
                    Vector2 dirB = new Vector2(-dirA.x, 0.5f);

                    a.ApplyKnockback(dirA, baseForce);
                    b.ApplyKnockback(dirB, baseForce);
                }
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
            winner.OnRam(dirA, loserForce);
            loser.OnRammed(winnerForce);
            
            // kmockback player of lower states
            if (winner.gameObject.GetComponent<PlayerManager>() is { } player)
            {
                var knockbackForce = player.playerStage switch
                {
                    PlayerStage.Teen => 0.7f,
                    PlayerStage.Adult => 0.3f,
                    _ => 1f // default for Young or any other unexpected value
                };
                
                var yForce = player.playerStage switch
                {   
                    PlayerStage.Teen => 0.5f,
                    PlayerStage.Adult => 0.35f,
                    _ => 1f // default for Young or any other unexpected value
                };

                Vector2 dir = (winner.transform.position - loser.transform.position).normalized;
                winner.ApplyKnockback(new Vector2(dir.x, yForce),  10);
                print($"APPLY KNOICKBACK to {new Vector2(dir.x, yForce)} with knockback {baseForce* knockbackForce} o0");
            }
            // Knockback loser
            Vector2 direction = (loser.transform.position - winner.transform.position).normalized;
            loser.ApplyKnockback(new Vector2(direction.x, 0.5f), (winnerForce-loserForce) * baseForce);
        }
    }

}