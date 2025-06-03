using System;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using Obstacles;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerManager : Rammer
    {
        private PlayerMovement _playerMovement;
        private PlayerHornDamageHandler _damageHandler;
        private PlayerSoundHandler soundHandler;
        private Npc followedBy;
        
        [SerializeField] private SpineControl spineControl;
        [SerializeField] private PlayerStage _playerStage = PlayerStage.Young;
        [SerializeField] private SpineControl _spineControl;

        private Rigidbody2D _rb;
        private bool isDead = false;
        private bool isKnockbacked = false;
        private bool inputEnabled = true;
        private float hitDamage = 0.5f;
        public bool InputEnabled => inputEnabled;
        public void DisableInput()
        {
            print("inpuut disabled!");
            inputEnabled = false;
        }

        public void EnableInput()
        {
            print("input enabled");
            inputEnabled = true;
        }

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
                _spineControl.changeSkelatonAnimation(_playerStage);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ApplyScaleForStage(_playerStage);
            _spineControl.changeSkelatonAnimation(_playerStage);
        }
#endif

        private void OnEnable()
        {
            print("player listen to cut scen");
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, OnEnterCutScene);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);  
        }

        public void UnlockAnimations()
        {
            spineControl.SetIdleLock(false);
        }
        
        public void LockAnimations()
        {
            spineControl.SetIdleLock(true);
        }
        private void OnEnterCutScene(object o)
        {
            if (o is string type)
            {
                print($"timeline tag is {type}");
                switch (type)
                {
                case "MeetBig":
                    
                    EnterMeetBigCutscene();
                    break;
                case "StartUpperground":
                    EnterStartUppergroundCutScene();
                    break;
                }
            }

        }

        private void EnterStartUppergroundCutScene()
        {
            DisableInput();
            LockAnimations();
        }

        private void EnterMeetBigCutscene()
        {
            DisableInput();
            print("enter cut scnee");
            
            LockAnimations();
            
            // turn elft
            var rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

        private void Start()
        {
            CurrentForce = 0;
            _rb = GetComponent<Rigidbody2D>();
            _playerMovement = GetComponent<PlayerMovement>();
            _damageHandler = GetComponent<PlayerHornDamageHandler>();

            if (CoreManager.Instance != null)
            {
                CoreManager.Instance.Player = this;
                print("player 9- in position");
            }

            ApplyScaleForStage(_playerStage);
            CoreManager.Instance.AudioManager.SetGlobalParameter("Evolution", 0);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var breakable = other.gameObject.GetComponent<IBreakable>();
            if (breakable is not null && _playerMovement.IsDashing)
            {
                breakable.OnHit((other.transform.position - transform.position).normalized, playerStage);
            }

            if (other.gameObject.GetComponent<IKillPlayer>() is not null)
            {
                print($"collided with {other.gameObject.name} 13");
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

                if (dot > 0.6f) // horizontal impact check
                {
                    RammerManager.Instance.ResolveRam(this, rammer);
                    print("{ram!! 987");
                }

                if (rammer.GetComponent<ChasingEnemy>() is not null)
                {
                    print("chasing enemy got me 76");
                    CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
                    // RammerManager.Instance.ResolveRam(this,rammer);
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
            print($"allow input {CoreManager.Instance.GameManager.AllowPlayerInput} isdead {isDead} Input enabled {InputEnabled}  rb vel {_rb.linearVelocity.magnitude}");
            if (CoreManager.Instance.GameManager.AllowPlayerInput && !isDead && !InputEnabled && _rb.linearVelocity.magnitude < 5f)
            {
                print("enable input!!");
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
            CoreManager.Instance.AudioManager.SetGlobalParameter("Evolution", (int) stage);

        }

        private void ApplyScaleForStage(PlayerStage stage)
        {
            Debug.Log($"Player stage: {stage}");
            transform.localScale = stage switch
            {
                PlayerStage.Young => new Vector3(0.83f, 0.83f, 1f),
                PlayerStage.Teen => new Vector3(1f, 1f, 1f),
                PlayerStage.Adult => new Vector3(1.2f, 1.2f, 1f),
                _ => transform.localScale
            };
            hitDamage = stage switch
            {
                PlayerStage.Teen => 0.25f,
                PlayerStage.Adult => 0.17f,
                _ => 0.5f
            }; 
        }

        // === Rammer Implementation ===
        public override void OnRam(Vector2 ramDirNegative, float againstForce)
        {
            Debug.Log($"Player rammed with force against {againstForce}");
            // CurrentForce = Mathf.Max(0, CurrentForce - againstForce * 0.5f);
            ApplyKnockback(ramDirNegative, againstForce/3);  // we are ramming, take 1/3 of the knockback
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Player got rammed with force {fromForce}");
            InjuryManager.Instance.ApplyDamage(hitDamage* fromForce);
            _damageHandler.AddDamage(hitDamage*100);
            spineControl.PlayAnimationOnBaseTrack("hit", false);

        }

        public IEnumerator DieAfterDelay()
        {
            DisableInput();
            isDead = true;
            print($"input enabled: {InputEnabled}");
            yield return new WaitForSeconds(2f);
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
            InjuryManager.Instance.Heal();
            EnableInput();
            print($"input enabled: {InputEnabled}");
            isDead = false;
            isKnockbacked = false;
            _rb.linearVelocity = Vector2.zero;

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

      

        public void Die()
        {
            StartCoroutine(DieAfterDelay());
        }

        public void GetMounted()
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            // turn right
            var rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
    }
    public enum PlayerStage
    {
        None,
        Young = 0,
        Teen = 1,
        Adult = 2,
    }
}
