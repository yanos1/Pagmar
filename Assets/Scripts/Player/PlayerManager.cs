using System;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using NPC;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Atmosphere.TileExplosion;
using Atmosphere.TileExplostion;
using Enemies;
using MoreMountains.Feedbacks;
using ScripableObjects;
using Spine.Unity;
using SpongeScene;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        [SerializeField] private PlayerSounds sounds;
        [SerializeField] private Transform finalStageLocation;
        
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
         private int healsPickedUp;
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
            _damageHandler.StartFullHeal(null);
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
            spineControl.PlayAnimation("idlev2",true,force: true);

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
            CoreManager.Instance.EventManager.AddListener(EventNames.BigDoingSomethingNice, OnBigDoingSomethingNice);
            CoreManager.Instance.EventManager.AddListener(EventNames.BigPickUpHeal, OnBigPickUpHeal);
        }

        private void OnDisable()
        {
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, OnEnterCutScene);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, EnableInputExternally);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.PlayerMeetSmall, StopAndDisableMovement);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.BigDoingSomethingNice, OnBigDoingSomethingNice);
            CoreManager.Instance.EventManager.RemoveListener(EventNames.BigPickUpHeal, OnBigPickUpHeal);

        }

        private void OnBigPickUpHeal(object obj)
        {
            if(++healsPickedUp < 2) return;
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(0.5f, () =>
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.laugh, transform.position);
                spineControl.PlayAnimation("look-up-smile",
                    3, loop: false, fallbackAnimation: "idlev2", force: true, onComplete: () =>
                    {
                        spineControl.ClearActionAnimation(3);
                    });
            }));        }

        private void OnBigDoingSomethingNice(object obj)
        {
            print("player smiles!");
            StartCoroutine(UtilityFunctions.WaitAndInvokeAction(1.8f, () =>
            {
                if(_playerMovement.isJumping) return;
                if (obj is Vector3 bigPos)
                {
                    Vector2 flipDir = bigPos.x < transform.position.x ? Vector2.left : Vector2.right;
                    _playerMovement.FlipSpecial(flipDir);

                }
                DisableInput();

                print($"sounds.laugh is {sounds.laugh.ToString()}");
                print($"Core manager audio : {CoreManager.Instance.AudioManager}");
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.laugh, transform.position);

                spineControl.PlayAnimation("look-up-smile",
                    3, loop: false, fallbackAnimation: "idlev2", force: true, onComplete: () =>
                    {
                        spineControl.ClearActionAnimation(3);
                        EnableInput();
                    });
            }));
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
                    case "MorningSleep":
                        print("morinig sleep activared");
                        DisableInput();
                        break;
                    case "Falldown":
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
                if (rammer.GetComponent<ChargingEnemy>() is not null && dot > 0.75f && isDead == false) // horizontal impact check
                {
                    print("resolve ram");
                    RammerManager.Instance.ResolveRam(this, rammer, other.GetContact(0).point + Vector2.up);
                }

                else if (rammer.GetComponent<ChasingEnemy>() is not null && isDead == false)
                {
                    print("player died");
                    Die(false);
                    RammerManager.Instance.ResolveRam(this, rammer, other.GetContact(0).point + Vector2.up); // flying enemy
                    // RammerManager.Instance.ResolveRam(this, rammer);
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
            // if (Input.GetKeyDown(KeyCode.G))
            // {
            //     SkipToTheEnd();
            // }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                Die();
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
                PlayerStage.FinalForm => 0.17f,
                _ => 0.5f
            };
        }

        // === Rammer Implementation ===
        public override void OnRam(Vector2 ramDirNegative, float againstForce)
        {
            Debug.Log($"Player rammed with force against {againstForce}");
            // CurrentForce = Mathf.Max(0, CurrentForce - againstForce * 0.5f);
            if (_playerStage != PlayerStage.FinalForm)
            {
                ApplyKnockback(ramDirNegative, againstForce / 3); // we are ramming, take 1/3 of the knockback
                InjuryFeedbacks.Instance.UpdateVisualFeedback();
            }
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.attackSound, transform.position);

        }

        public override void OnRammed(float fromForce, Vector3 collisionPoint)
        {
            Debug.Log($"Player got rammed with force {fromForce}");
            var clashPart = CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.EnemyHitPlayerParticles);
            if (clashPart is not null)
            {
                print($"alert: {{particles are  {clashPart}");
                clashPart.Play(collisionPoint +Vector3.left*0.5f);
            }
            else
            {
                print("alert: particles got are NULL");
            }
            
            InjuryFeedbacks.Instance.UpdateVisualFeedback(true);

            spineControl.PlayAnimationOnBaseTrack("hit", false);
            if (Mathf.Approximately(fromForce, 1))
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.damagedSound, transform.position);

            }
            else
            {
                CoreManager.Instance.AudioManager.PlayOneShot(sounds.damagedHardSound, transform.position);
            }
            OnRammedFeedback();
            if(isDead) return;
            if (_playerStage == PlayerStage.FinalForm)
            {
                _damageHandler.AddDamage(1);
            }
            else
            {
                _damageHandler.AddDamage(2);
            }

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

        public override void OnTie(float fromForce, Vector3 collisionPoint)
        {
            var clashPart = CoreManager.Instance.PoolManager.GetFromPool<ParticleSpawn>(PoolEnum.ClashParticles);
            clashPart.Play(collisionPoint);
            _damageHandler.AddDamage(1);
            CoreManager.Instance.AudioManager.PlayOneShot(sounds.clashSound, transform.position);
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

            if (curLife >= 5)
            {
                currentlyPlayingShake = LowCameraShakeFeedBack;
            }
            else if (curLife >= 3)
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
            if (ramComboCount > 1 && !liftFeedbacks.IsPlaying &&
                CoreManager.Instance.GameManager.InCutScene) // this is horrilbe code. i just dont ahve time.
            {
                print("lift player!");
                liftFeedbacks?.PlayFeedbacks();
                isKnockbacked = false;
                return;
            }
            else if (liftFeedbacks.IsPlaying)
            {
                print("return without lifting, end knockback");
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
                PlayerStage.Teen => 0f,
                PlayerStage.Adult => 1f,
                PlayerStage.FinalForm => 2f,
            };
        }

        public override void ResetForce()
        {
            CurrentForce = 0f;
        }

        public void Die(bool lockAnimationsAfterDeath = true)
        {
            if (isDead) return;
            print("die!123");
            isDead = true;
            isKnockbacked = false;
            if (lockAnimationsAfterDeath) LockAnimations();
            CoreManager.Instance.EventManager.InvokeEvent(EventNames.Die, null);
        }

        public void Revive()
        {
            isDead = false;
            _playerMovement.StopAllMovement(null);
            spineControl.PlayAnimation("idlev2",true,force: true);
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
            _playerMovement.StopAllMovement(null);
        }

        public void DisableInputForDuration(float seconds)
        {
            StartCoroutine(DisableInputForDurationCoroutine(seconds));
        }

        private IEnumerator DisableInputForDurationCoroutine(float seconds)
        {
            yield return
                new WaitUntil(() =>
                    isDead == false); //reset maanger will give input back after player died, we wait and then disable it for 4 seconds
            DisableInput();
            yield return new WaitForSeconds(seconds);
            EnableInput();
        }

        public void ResetToInitialState()
        {
            StopAllCoroutines();
            ChangeToOriginalColor();
            EnableInput();
            isKnockbacked = false;
            // if (_playerMovement.isWallSliding)
            // {
            //     print("force flip" );
            //     _playerMovement.ForcePlayerFlip();
            // }
        }

        private void ChangeToOriginalColor()
        {
            doSpineFlash.RestoreOriginalColors();
        }

        public void OnSkipToTheEnd(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (SceneManager.GetActiveScene().name != "Underground") return;
                SetPlayerStage(PlayerStage.FinalForm);
                _spineControl.changeSkelatonAnimation(_playerStage);
                ChangeHitDamage(_playerStage);
                SetForce();
                _playerMovement.GameObject().transform.position = finalStageLocation.localPosition;
            }

        }
    }
}

public enum PlayerStage
{
    None = -1,
    Young = 0,
    Teen = 1,
    Adult = 2,
    FinalForm = 3,
}