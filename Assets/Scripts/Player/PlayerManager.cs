using System;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using NPC.NpcActions;
using Obstacles.Shooters.Projectiles;
using Terrain;
using Unity.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private Npc followedBy;
        private void Start()
        {
            CoreManager.Instance.Player = this;
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<IBreakable>() is { } breakable && _playerMovement.IsDashing)
            {
                breakable.OnHit((other.transform.position - gameObject.transform.position).normalized);
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            }
            
            if (other.gameObject.GetComponent<Projectile>() is { } proj)
            {
                if (proj.IsDeadlyProjectile())
                {
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
                }
            }
        }

        public void SetFollowedBy([CanBeNull] Npc npc)
        {
            followedBy = npc;
        }

        public Npc GetFollowedBy()
        {
            return followedBy;
        }
    }
}