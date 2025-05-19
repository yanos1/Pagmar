using System;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using UnityEngine;
using System.Collections;

namespace Player
{
    public class PlayerManager : Rammer
    {
        private PlayerMovement _playerMovement;
        private Npc followedBy;
        [SerializeField] private SpineControl spineControl;
        [SerializeField] private PlayerStage _playerStage = PlayerStage.Young;
        [SerializeField] private float knockbackStrength = 50f;
        [SerializeField] private float injuredDuration = 5f;

        private Rigidbody2D _rb;
        private Coroutine injuryCoroutine;
        private bool isInjured = false;
        private bool isDead = false;
        private bool isKnockbacked = false;
        private bool inputEnabled = true;
        public bool InputEnabled => inputEnabled;
        public void DisableInput() => inputEnabled = false;
        public void EnableInput() => inputEnabled = true;
        public bool IsInjured => isInjured;
        public bool IsKnockBacked => isKnockbacked;
        
        public bool IsDead => isDead;


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
            
            CurrentForce = 0;
            _rb = GetComponent<Rigidbody2D>();
            _playerMovement = GetComponent<PlayerMovement>();

            if (CoreManager.Instance != null)
            {
                CoreManager.Instance.Player = this;
                print("player 9- in position");
            }

            ApplyScaleForStage(_playerStage);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var breakable = other.gameObject.GetComponent<IBreakable>();
            if (breakable is not null && _playerMovement.IsDashing)
            {
                breakable.OnHit((other.transform.position - transform.position).normalized);
            }

            if (other.gameObject.GetComponent<IKillPlayer>() is { } killPlayer && killPlayer.IsDeadly())
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
                return;
            }

            CheckForRam(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            CheckForRam(other);
        }

        private void CheckForRam(Collision2D other)
        {
            if (other.gameObject.GetComponent<Rammer>() is { } rammer)
            {
                Vector2 directionToPlayer = (transform.position - rammer.transform.position).normalized;
                float dot = Mathf.Abs(Vector2.Dot(directionToPlayer, Vector2.right));

                if (dot > 0.5f) // horizontal impact check
                {
                    RammerManager.Instance.ResolveRam(this, rammer);
                    print("{ram!! 987");
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<IKillPlayer>() is { } killPlayer && killPlayer.IsDeadly())
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.F12))
            {
                CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            }

            else if (Input.GetKey(KeyCode.F1))
            {
                _playerStage = PlayerStage.Teen;
            }
            else if (Input.GetKey(KeyCode.F2))
            {
                _playerStage = PlayerStage.Adult;
            }
            
            if (!isDead && !InputEnabled && _rb.linearVelocity.magnitude < 5f)
            {
                EnableInput();
                isKnockbacked = false;

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
            if (stage == PlayerStage.Teen)
            {
                _playerMovement.enableAdvancedDash = true;
            }
        }

        private void ApplyScaleForStage(PlayerStage stage)
        {
            Debug.Log($"Player stage: {stage}");
            transform.localScale = stage switch
            {
                PlayerStage.Young => new Vector3(0.83f, 0.83f, 1f),
                PlayerStage.Teen => new Vector3(1f, 1f, 1f),
                PlayerStage.Adult => new Vector3(1.5f, 1.5f, 1f),
                _ => transform.localScale
            };
        }

        // === Rammer Implementation ===
        public override void OnRam(float againstForce)
        {
            Debug.Log($"Player rammed with force against {againstForce}");
            // CurrentForce = Mathf.Max(0, CurrentForce - againstForce * 0.5f);
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Player got rammed with force {fromForce}");
            spineControl.PlayAnimationOnBaseTrack("hit", false);
            if (isInjured)
            {
                StartCoroutine(DieAfterDelay());
                return;
            }

            if (injuryCoroutine != null)
                StopCoroutine(injuryCoroutine);

            injuryCoroutine = StartCoroutine(InjuryTimer());
        }

        private IEnumerator DieAfterDelay()
        {
            DisableInput();
            isDead = true;
            print($"input enabled: {InputEnabled}");
            yield return new WaitForSeconds(2f);
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            EnableInput();
            print($"input enabled: {InputEnabled}");
            isInjured = false;
            isDead = false;
            isKnockbacked = false;
            _rb.linearVelocity = Vector2.zero;

        }

        private IEnumerator InjuryTimer()
        {
            isInjured = true;
            Debug.Log("Player is injured!");

            yield return new WaitForSeconds(injuredDuration);

            isInjured = false;
            injuryCoroutine = null;
            Debug.Log("Player has recovered from injury.");
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {            
            DisableInput();
            isKnockbacked = true;
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
            Debug.Log($"Add force on player: {force} in dir {direction.normalized}");
            
        }

        public void SetForce()
        {
            CurrentForce = playerStage switch
            {
                PlayerStage.Young => 0f,
                PlayerStage.Teen => 1f,
                PlayerStage.Adult => 2f,
                _ => 0f
            };
        }

        public override void ResetForce()
        {
            CurrentForce = 0f;
        }

        public enum PlayerStage
        {
            None,
            Young,
            Teen,
            Adult,
        }
    }
}
