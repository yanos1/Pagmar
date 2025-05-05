using System;
using Enemies;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using NPC.NpcActions;
using Obstacles.Shooters.Projectiles;
using Terrain;
using Terrain.Environment;
using Unity.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private Npc followedBy;

        [SerializeField] 
        private PlayerStage _playerStage = PlayerStage.Young;

        public PlayerStage playerStage
        {
            get => _playerStage;
            set
            {
                if (_playerStage == value) return;
                _playerStage = value;
                ApplyScaleForStage(_playerStage);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyScaleForStage(_playerStage);
        }
#endif

        private void Start()
        {
            CoreManager.Instance.Player = this;
            _playerMovement = GetComponent<PlayerMovement>();
            ApplyScaleForStage(_playerStage);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.GetComponent<IBreakable>() is { } breakable && _playerMovement.IsDashing)
            {
                breakable.OnHit((other.transform.position - gameObject.transform.position).normalized);
            }

            if (other.gameObject.GetComponent<Enemy>() is { } enemy && enemy.IsDeadly())
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
            else if (other.gameObject.GetComponent<FallingStone>() is { } stone)
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
                stone.HitPlayer();
            }
 
        
        }

        public void SetFollowedBy([CanBeNull] Npc npc)
        {
            followedBy = npc;
        }

        public Npc GetFollowedBy() => followedBy;
        
        

        public void SetPlayerStage(PlayerStage stage)
        {
            playerStage = stage;
        }

        private void ApplyScaleForStage(PlayerStage stage)
        {
            Debug.Log($"Player stage: {stage}");
            transform.localScale = stage switch
            {
                PlayerStage.Young  => new Vector3(0.83f,    0.83f,    1f),
                PlayerStage.Teen   => new Vector3(1f, 1f, 1f),
                PlayerStage.Adult  => new Vector3(1.5f,  1.5f,  1f),
                PlayerStage.Elder  => new Vector3(1.25f, 1.25f, 1f),
                _                  => transform.localScale
            };
        }

        public enum PlayerStage
        {
            Young,
            Teen,
            Adult,
            Elder
        }
    }
}
