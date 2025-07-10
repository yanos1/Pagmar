using System;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using MoreMountains.Feedbacks;
using Spine.Unity;

namespace Player
{
    public class PlayerManager : Rammer, IResettable
    {
        private PlayerMovement _playerMovement;
        private PlayerHornDamageHandler _damageHandler;
        private PlayerSoundHandler soundHandler;
        private Npc followedBy;


        [SerializeField] private MMF_Player liftFeedbacks;

        [SerializeField] private SpineControl spineControl;
        [SerializeField] private PlayerStage _playerStage = PlayerStage.Young;
        [SerializeField] private SpineControl _spineControl;
        [SerializeField] private MMF_Player LowCameraShakeFeedBack;
        [SerializeField] private MMF_Player MediumCameraShakeFeedBack;
        [SerializeField] private MMF_Player HighCameraShakeFeedBack;
        [SerializeField] private DoSpineFlash doSpineFlash;
        [SerializeField] private GameObject light;
        private MMF_Player currentlyPlayingShake;



        private Rigidbody2D _rb;
        private bool isDead = false;
        private bool isKnockbacked = false;
        private bool inputEnabled = true;
        private float hitDamage = 0.5f;

        private float lastRammedTime = -1f;
        private int ramComboCount = 0;
        private const float comboTimeWindow = 0.5f;

        private bool isGodMode = false;
        public bool InputEnabled => inputEnabled;
        public bool IsMoving => Mathf.Abs(_rb.linearVelocity.x) > 0.2;

        public bool IsGodMode => isGodMode;
        

        public void DisableInput()
        {
            print("inpuut disabled!");
            inputEnabled = false;
        }

        public void EnableInput()
        {
            print("input enabled");
            UnlockAnimations(); // this is a palce holder.
            inputEnabled = true;
        }

        public void EnterGodMode()
        {
            isGodMode = true;
            light.SetActive(true);
        }

        public void ExitGodMode()
        {
            isGodMode = false;
            light.SetActive(false);
        }
        
        public void StopAndDisableMovement(object o)
        {
            DisableInput();
            _playerMovement.StopAllMovement(null);
        }

        public bool IsKnockBacked => isKnockbacked;

        public bool IsDead => isDead;

        public PlayerStage playerStage
        {
            get => _playerStage;
            set
            {
                // if (_playerStage == value) return;
                _playerStage = value;
                ChangeHitDamage(_playerStage);
                _spineControl.changeSkelatonAnimation(_playerStage);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ChangeHitDamage(_playerStage);
            _spineControl.changeSkelatonAnimation(_playerStage);
        }
#endif

        private void OnEnable()
        {
            print("player listen to cut scen");
            playerStage = playerStage; // swaps animation at start for debug.
            CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, OnEnterCutScene);
            CoreManager.Instance.EventManager.AddListener(EventNames.EndCutScene, EnableInputExternally);
            CoreManager.Instance.EventManager.AddListener(EventNames.PlayerMeetSmall, StopAndDisableMovement);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, EnableInputExternally);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PlayerMeetSmall, StopAndDisableMovement);
        }

        public void EnableInputExternally(object o)
        {
            if (o is bool enableInput)
            {
                if (enableInput)
                {
                    print("enable input");
                    EnableInput();
                }
                else
                {
                    print($"enable input is {enableInput} so non input");
                }
            }
        }
        public void UnlockAnimations()
        {
            spineControl.SetIdleLock(false);
        }

        public void LockAnimations()
        {
            spineControl.SetIdleLock(true);
        }

        public void UpgradeState()
        {
            int next = (int)playerStage + 1;

            SetPlayerStage((PlayerStage)next);
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
                    case "HouseCollapse":
                        DisableInput();
                        break;
                    default:
                        DisableInput();
                        LockAnimations();
                        break;
                }
            }
            else
            {
                DisableInput();
                LockAnimations();
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

            ChangeHitDamage(_playerStage);
            CoreManager.Instance.AudioManager.SetGlobalParameter("Evolution", 0);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {

            if (other.gameObject.GetComponent<IKillPlayer>() is { } killPlayer && killPlayer.IsDeadly())
            {
                Die();
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
                print($"hit rammer with dot : {dot}");
                if (rammer.GetComponent<ChargingEnemy>() is not null && dot > 0.75f) // horizontal impact check
                {
                    RammerManager.Instance.ResolveRam(this, rammer);
                }

                else if (rammer.GetComponent<ChasingEnemy>() is not null && isDead == false)
                {
                    print("player died");
                    Die();
                    // RammerManager.Instance.ResolveRam(this, rammer);
                }

                else if (rammer.GetComponent<ChargingEnemy>() is null)
                {
                    RammerManager.Instance.ResolveRam(this, rammer); // flying enemy
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<IKillPlayer>() is { } killPlayer && killPlayer.IsDeadly())
            {
                Die();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                OnRammedFeedback();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                Die();
            }

            else if (Input.GetKeyDown(KeyCode.F1))
            {
                _playerStage = PlayerStage.Teen;
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                _playerStage = PlayerStage.Adult;
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
            print($"set stage to {playerStage}");
            CoreManager.Instance.AudioManager.SetGlobalParameter("Evolution", (int)stage);
        }

        private void ChangeHitDamage(PlayerStage stage)
        {
            Debug.Log($"Player stage: {stage}");
            transform.localScale = stage switch
            {
                // PlayerStage.Young => new Vector3(0.83f, 0.83f, 1f),
                // PlayerStage.Teen => new Vector3(1f, 1f, 1f),
                PlayerStage.Adult => new Vector3(1f, 1f, 1f),
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
            if (_playerStage != PlayerStage.Adult)
            {
                ApplyKnockback(ramDirNegative, againstForce / 3); // we are ramming, take 1/3 of the knockback
                InjuryFeedbacks.Instance.UpdateVisualFeedback();
            }
        }

        public override void OnRammed(float fromForce)
        {
            Debug.Log($"Player got rammed with force {fromForce}");
            if (_playerStage == PlayerStage.Adult)
            {
                _damageHandler.AddDamage(1);
            }
            else
            {
                _damageHandler.AddDamage(2);
            }
            InjuryFeedbacks.Instance.UpdateVisualFeedback(true);

            spineControl.PlayAnimationOnBaseTrack("hit", false);
            OnRammedFeedback();

            float currentTime = Time.time;

            // Check if current ram is within combo window
            if (currentTime - lastRammedTime <= comboTimeWindow)
            {
                ramComboCount++;
            }
            else
            {
                ramComboCount = 1;
            }

            lastRammedTime = currentTime;

        }

        public override void OnTie(float fromForce)
        {
            _damageHandler.AddDamage(1);
            InjuryFeedbacks.Instance.UpdateVisualFeedback();

            spineControl.PlayAnimationOnBaseTrack("hit", false);
            OnRammedFeedback();

            float currentTime = Time.time;
        }

        private void OnRammedFeedback()
        {
            doSpineFlash.OnRammedFeedback();
            StartCoroutine(DoTimeFreeze(0.07f));
            MakeScreenShakeDependingOnLife();
        }

        private void MakeScreenShakeDependingOnLife()
        {
            if (currentlyPlayingShake != null && currentlyPlayingShake.IsPlaying)
                return;

            var curLife = _damageHandler.currentIndex;

            if (curLife >=5 )
            {
                currentlyPlayingShake = LowCameraShakeFeedBack;
            }
            else if (curLife >=3)
            {
                currentlyPlayingShake = MediumCameraShakeFeedBack;
            }
            else
            {
                currentlyPlayingShake = HighCameraShakeFeedBack;
            }

            currentlyPlayingShake.PlayFeedbacks();

            StartCoroutine(ClearShakeAfterFeedback(currentlyPlayingShake));
        }

        private IEnumerator ClearShakeAfterFeedback(MMF_Player shake)
        {
            yield return new WaitUntil(() => !shake.IsPlaying);

            if (currentlyPlayingShake == shake)
            {
                currentlyPlayingShake = null;
            }
        }


        private IEnumerator DoTimeFreeze(float time)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(time);
            Time.timeScale = 1f;
        }


        public override void ApplyKnockback(Vector2 direction, float force)
        {
            if (ramComboCount > 1 && !liftFeedbacks.IsPlaying && CoreManager.Instance.GameManager.InCutScene)  // this is horrilbe code. i just dont ahve time.
            {
                liftFeedbacks?.PlayFeedbacks();
                isKnockbacked = false;
                return;
            } else if (liftFeedbacks.IsPlaying)
            {
                return;
            }
            isKnockbacked = true;
            if (inputEnabled)
            {
                DisableInput();
                StartCoroutine(ReturnInputAfterRammed());
            }
       

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
            if(isDead) return;
            isDead = true;
            isKnockbacked = false;
            LockAnimations();
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
        }

        public void Revive()
        {
            isDead = false;
            _playerMovement.StopAllMovement(null);
        }

    public void GetMounted()
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            // turn right
            var rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

        private IEnumerator ReturnInputAfterRammed()
        {
            while (_rb.linearVelocity.magnitude >= 5f)
            {
                yield return null;
            }

            EnableInput();
            isKnockbacked = false;
        }

        public void StopAllMovement()
        {
            _rb.linearVelocity = Vector2.zero;
        }

        public void DisableInputForDuration(float seconds)
        {
            StartCoroutine(DisableInputForDurationCoroutine( seconds));
        }

        private IEnumerator DisableInputForDurationCoroutine(float seconds)
        {
            yield return new WaitUntil(() => isDead == false); //reset maanger will give input back after player died, we wait and then disable it for 4 seconds
            DisableInput();
            yield return new WaitForSeconds(seconds);
            EnableInput();
        }

        public void ResetToInitialState()
        {
            StopAllCoroutines();
            ChangeToOriginalColor();
            Revive();
            EnableInput();
            isDead = false;
            isKnockbacked = false;
        }

        private void ChangeToOriginalColor()
        {
            doSpineFlash.RestoreOriginalColors();
        }
    }
}

public enum PlayerStage
{
    None = -1,
    Young = 0,
    Teen = 1,
    Adult = 2,
}