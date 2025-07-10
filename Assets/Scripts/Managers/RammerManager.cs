using System;
using System.Collections.Generic;
using Interfaces;
using MoreMountains.Feedbacks;
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
        [SerializeField] private MMF_Player playerHitFeedback;

        private Dictionary<Rammer, ValueTuple<Rammer, float>> rammersHistory = new();
        private float mintimeBetweenRams = 0.6f;

        private void Awake()
        {
            Instance = this;
        }


        private void Update()
        {
            if (Input.GetKey(KeyCode.K))
            {
                playerHitFeedback.PlayFeedbacks();
            }
            if (rammersHistory.Count  == 0) return;
            var removes = new List<Rammer>();
            foreach (var (key, pair) in rammersHistory)
            {
                if (Time.time - pair.Item2 > mintimeBetweenRams)
                {
                    removes.Add(key);
                }
            }

            foreach (var rammer in removes)
            {
                rammersHistory.Remove(rammer);
            }
        }

        public void ResolveRam(Rammer a, Rammer b)
        {
            if (rammersHistory.GetValueOrDefault(a).Item1 == b)
            {
                return; // not enough time passed to do a second ram.
            }

            rammersHistory[a] = (b, Time.time);
            float forceA = a.CurrentForce;
            float forceB = b.CurrentForce;
           
            print($"{a.CurrentForce} {b.CurrentForce}");
            
            Vector2 dirA = (a.transform.position - b.transform.position).normalized;
            if (forceA ==0 && forceB == 0 && a.IsCharging) 
            {
                a.ApplyKnockback(dirA, baseForce*0.7f);

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
                    playerHitFeedback?.PlayFeedbacks();
                    a.ApplyKnockback(dirA, baseForce);
                    b.ApplyKnockback(dirB, baseForce);
                    a.OnTie(b.CurrentForce);
                    b.OnTie(a.CurrentForce);
                }
                return;
            }

            // Determine winner and loser
            Rammer winner = forceA > forceB ? a : b;
            Rammer loser = forceA > forceB ? b : a;
            // print($"rammer is {winner} 55 ");
            // print($"rammed is {loser} 55");
            float winnerForce = Mathf.Max(forceA, forceB);
            float loserForce = Mathf.Min(forceA, forceB);

            // Winner rams, loser gets rammed
            winner.OnRam(dirA, loserForce);
            loser.OnRammed(winnerForce);
            
            // kmockback player of lower states
            Vector2 loseDir = (loser.transform.position - winner.transform.position).normalized;
            if (winner.gameObject.GetComponent<PlayerManager>() is { } player)
            {
                var knockbackForce = player.playerStage switch
                {
                    PlayerStage.Teen => 0.7f,
                    PlayerStage.Adult => 0.3f,
                    PlayerStage.FinalForm => 0.3f,
                    _ => 1f // default for Young or any other unexpected value
                };
                
                var yForce = player.playerStage switch
                {   
                    PlayerStage.Teen => 0.5f,
                    PlayerStage.Adult => 0.35f,
                    PlayerStage.FinalForm => 0.35f,
                    _ => 1f // default for Young or any other unexpected value
                };

                Vector2 winDir = (winner.transform.position - loser.transform.position).normalized;
                if (player.playerStage != PlayerStage.FinalForm)
                {
                    winner.ApplyKnockback(new Vector2(winDir.x, yForce),  10);
                }
               
                print($"APPLY KNOICKBACK to {new Vector2(winDir.x, yForce)} with knockback {baseForce* knockbackForce} o0");
                loser.ApplyKnockback(new Vector2(loseDir.x, 0.5f),(winnerForce-loserForce) *baseForce/2);
                playerHitFeedback?.PlayFeedbacks();
                return;
            }
            // Knockback loser
            loser.ApplyKnockback(new Vector2(loseDir.x, 0.25f), baseForce);
        }
    }

}