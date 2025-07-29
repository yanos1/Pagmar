using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using FMOD.Studio;
using FMODUnity;
using Interfaces;
using Managers;
using MoreMountains.Feedbacks;
using Player;
using ScripableObjects;
using Spine;
using Spine.Unity;
using SpongeScene;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using AnimationState = UnityEngine.AnimationState;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PlayerMovement : MonoBehaviour, IResettable
{
    [Header("Movement")] [SerializeField] private float MovementSpeed = 200f;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Jump")] [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float regularGravity = 1.2f;
    [SerializeField] private float WhenStopPressGravity = 2.5f;
    [SerializeField] private float maxFallingSpeed = -10f;
    [SerializeField] private float graceJumpTime = 0.1f;
    [SerializeField] private float minJumpInterval = 0.30f;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheckPosition;

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private List<LayerMask> groundLayers;

    [Header("Wall Jump & Slide")] [SerializeField]
    private Transform wallCheckPosition;

    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpForceX = 10f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float wallJumpGravity = 0.8f;

    [Header("Dash")] [SerializeField] private float dashSpeed = 150f;
    [SerializeField] private float dashAttackTime = 0.15f;
    [SerializeField] private float dashEndTime = 0.15f;
    [SerializeField] private float dashEndSpeed = 50f;
    [SerializeField] private float dashCoolDownTime = 1f;
    public bool enableDash = true;
    public bool enableAdvancedDash = false;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool isWalkingScene;
    [SerializeField] private Transform finalFormSpine;


    [Header("CammeraFollowObject")] [SerializeField]
    private CameraFollowObject _cameraFollowObject;

    [SerializeField] private PlayerSounds playerSounds;
    [SerializeField] private Transform cielingCheckPos;
    private PlayerSoundHandler playerSoundHandler;


    public bool isJumping = false;
    public bool jumpIsPressed = false;
    private float LastPressedJumpTime = 0f;
    private float LastOnGroundTime = 0f;
    private bool _isFacingRight = true;
    private float _moveInputX;
    private float _moveInputY;
    private Coroutine jumpTimer;
    private bool _isDashAttacking = false;
    private bool _isDashing = false;
    private Vector2 _lastDashDir;
    private Vector2 _moveInput;
    private float LastPressedDashTime;
    private bool _canDash = true;
    private float _fallSpeedYDampingChangeThreshold;
    private bool hitSomethingDuringDash;
    private float lastMoveTime;
    public bool IsFacingRight => _isFacingRight;
    public bool IsDashing => _isDashing;
    public Vector2 DashDirection => _lastDashDir;
    public Vector3 GroundCheckPos => groundCheckPosition.position;
    public float LastJumpTime => LastPressedJumpTime;
    public float LastMoveTime => lastMoveTime;

    private bool isTouchingWall = false;
    private bool isWallJumping = false;
    private float wallJumpDirection;
    private float wallJumpCounter;
    private bool hasWallJumped = false;
    public bool isWallSliding = false;
    private bool _wasGroundedLastFrame = false;
    private bool _playedStartJump = false;
    private bool _preventAnimOverride = false;
    private bool isFalling;
    private float timeFalling;
    private bool _isPlayingWallJumpLand = false;
    
    private bool enteredInput = false;
    private bool cutSceneEnded = false;
    private bool makeNextfallHard;



    [Header("MM Feedback")] [SerializeField]
    private MMF_Player jumpFeedback;

    [SerializeField] private MMF_Player landFeedback;
    [SerializeField] private MMF_Player dashFeedback;
    [SerializeField] public bool enableWallJump;
    private PlayerManager player;
    private PlayerHornDamageHandler hornDamageHandler;
    [SerializeField] SpineControl spineControl;
    private Coroutine dashHitCoroutine;
    private EventInstance fallInstance;

    private void OnEnable()
    {
        CoreManager.Instance.EventManager.AddListener(EventNames.EnterCutScene, StopAllMovement);
        CoreManager.Instance.EventManager.AddListener(EventNames.StartLoadNextScene, OnLoadNewScene);
        CoreManager.Instance.EventManager.AddListener(EventNames.EndCutScene, OnEndCutScene);
        CoreManager.Instance.EventManager.AddListener(EventNames.ReachedFirstObstacle, StopScaredEyes);
        lastMoveTime = Time.time;
        // CoreManager.Instance.EventManager.AddListener(EventNames.StartNewScene, OnNewScene);
    }

    private void OnDisable()
    {
        CoreManager.Instance.EventManager.RemoveListener(EventNames.StartLoadNextScene, OnLoadNewScene);
        CoreManager.Instance.EventManager.RemoveListener(EventNames.EnterCutScene, StopAllMovement);
        CoreManager.Instance.EventManager.RemoveListener(EventNames.EndCutScene, OnEndCutScene);
        CoreManager.Instance.EventManager.RemoveListener(EventNames.ReachedFirstObstacle, StopScaredEyes);


        // CoreManager.Instance.EventManager.RemoveListener(EventNames.StartNewScene, OnNewScene);

    }


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        LastOnGroundTime = Time.time;
        LastPressedDashTime = Time.time;
        _isFacingRight = true;
        player = GetComponent<PlayerManager>();
        playerSoundHandler = GetComponent<PlayerSoundHandler>();
        hornDamageHandler = GetComponent<PlayerHornDamageHandler>();
    }

    private void Start()
    {
        _rb.gravityScale = regularGravity;
        _fallSpeedYDampingChangeThreshold = CameraManager.GetInstance().FallSpeedYDampingChangeThreshold;
        _isFacingRight = true;
    }

    private void Update()
    {
        LastOnGroundTime += Time.deltaTime;
        if (!_isDashAttacking && !isWallJumping && !isWallSliding)
            Move();

        CheckIfGrounded();
        CheckIfFalling();

        isTouchingWall = IsTouchingWall();
        WallSlide();

        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0) 
            {
                isWallJumping = false;
                _rb.gravityScale = regularGravity;
            }
        }

        if (isFalling)
        {
            fallInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        }

        UpdateAnimation();
    }
    
    private void OnEndCutScene(object obj)
    {
        cutSceneEnded = true;
        StartCoroutine(UtilityFunctions.WaitAndInvokeAction(1f, () => cutSceneEnded = false));
    }


    public void Sleep()
    {
        print("player is sleeping");
        _preventAnimOverride = true;
        // Set "sleep" animation on track 3 with looping
        spineControl.PlayAnimation("sleep", 3, loop: true, force: true, fallbackAnimation: null);
    }

    public void WakeUp()
    {
        StartCoroutine(WakeUpRoutine());
    }

    private IEnumerator WakeUpRoutine()
    {
        // Wait until any input is pressed
        print("player is waking up");
        yield return new WaitUntil(() =>
            Keyboard.current?.anyKey.wasPressedThisFrame == true ||
            Gamepad.current?.buttonSouth.wasPressedThisFrame == true ||
            Gamepad.current?.leftStick.ReadValue().magnitude > 0.1f || cutSceneEnded
        );

        if (cutSceneEnded)
        {
            if (ScenesManager.Instance.CurrentScene == 5)  // wake up with big
            {
                CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.wakeUp, transform.position);
            }
            spineControl.PlayAnimation("wake-up", 3, loop: false, force: true, fallbackAnimation: null, onComplete:
                () =>
                {
                    // spineControl.ClearActionAnimation(4);
                    spineControl.ClearActionAnimation(3);

                    _preventAnimOverride = false;
                    player.EnableInput();
                });
            yield break;
        }
        // Crossfade from sleep to wake-up (track 3)
        var skeletonAnimation = spineControl.skeletonAnimation;
        var state = skeletonAnimation.AnimationState;
        var stateData = state.Data;

// Optional: Define mix duration from wake-up → wake-up-jump
        stateData.SetMix("sleep", "wake-up", 4f);

// Get the duration of the first animation
        float wakeUpDuration = skeletonAnimation.Skeleton.Data.FindAnimation("wake-up").Duration;

// Play "wake-up" first
        state.SetAnimation(3, "wake-up", false);

// Queue "wake-up-jump" to start after "wake-up" finishes
        var jumpEntry = state.AddAnimation(3, "wake-up-jump", false, wakeUpDuration/1.5f);
        jumpEntry.Complete += entry =>
        {
            spineControl.ClearActionAnimation(3);
            _preventAnimOverride = false;
            player.EnableInput();
        };


    }

    public void MakeNextFallHard()
    {
        makeNextfallHard = true;
    }

    public void WakeUpFromFrom()
    {
        spineControl.PlayAnimation("scared-eyes", 4, force: true, loop: true);
        spineControl.PlayAnimation("fall-getup",3, force:true, onComplete: (() =>
        {
            spineControl.ClearActionAnimation(3);
            player.EnableInput();

        }));
    }
    
    private void StopScaredEyes(object obj)
    {
        print("stop scared eyes!");
        spineControl.ClearActionAnimation(4);
    }
    
    public void StopAllMovement(object obj)
    {
        _rb.linearVelocity = Vector2.zero;
        _moveInputX = 0;
        _moveInputY = 0;
        _moveInput = Vector2.zero;
        _isDashing = false;
        print("stopped all movement!");
        // spineControl.PlayAnimation("idlev2",true,force: true);
    }
    
    private void OnLoadNewScene(object obj)
    {
        fallInstance.stop(STOP_MODE.ALLOWFADEOUT);
        CoreManager.Instance.AudioManager.RemoveSoundToStopWhenGoingToMainMenu(fallInstance);
        fallInstance.release();
    }
    
    // private void OnNewScene(object obj)
    // {
    //     if (SceneManager.GetActiveScene().name == "Falldown")
    //     {
    //         StartCoroutine(MoveRightForSeconds());
    //     
    //     }
    // }

    // private IEnumerator MoveRightForSeconds()
    // {
    //     _moveInputX = 0.79876f;
    //     _moveInputY = 0;
    //     _moveInput = Vector2.right;
    //     yield return new WaitForSeconds(2);
    //     if (Mathf.Approximately(_moveInputX, 0.79876f)) // if the player hasnt touchd the input this will be the input and we will stop
    //     {
    //         _moveInputX = 0;
    //         _moveInput.y = 0;
    //         _moveInput = Vector2.right;
    //     }
    // }

    private void CheckIfFalling()
    {
        if (_rb.linearVelocity.y < _fallSpeedYDampingChangeThreshold &&
            !CameraManager.GetInstance().IsLerpingYDamping && !CameraManager.GetInstance().LerpedFromPlayerFalling)
        {
            CameraManager.GetInstance().LerpYDamping(true);
        }

        if (_rb.linearVelocity.y < -0.4f && !player.IsKnockBacked)
        {
            if (!isFalling)
            {
                print("call fall sound");
                 fallInstance = CoreManager.Instance.AudioManager.CreateEventInstance(playerSounds.fallSound);
                 fallInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                 CoreManager.Instance.AudioManager.RegisterSoundToStopWhenGoingToMainMenu(fallInstance);

                 fallInstance.start();
            }

            isFalling = true;
        }

        if (isFalling)
        {
            timeFalling += Time.deltaTime;
        }

        if (_rb.linearVelocity.y >= 0 &&
            !CameraManager.GetInstance().IsLerpingYDamping && CameraManager.GetInstance().LerpedFromPlayerFalling)
        {
            CameraManager.GetInstance().LerpedFromPlayerFalling = false;
            CameraManager.GetInstance().LerpYDamping(false);
        }

        if (isFalling && _rb.linearVelocity.y >= -0.12)
        {
            print($"time falling {timeFalling}");
            // if (timeFalling > 2.5)
            // {
            //     print($"take damaeh");
            //
            //     InjuryManager.Instance.ApplyDamage(0.5f); // this is just effects of damage (slow, screen turns white..)
            //     hornDamageHandler.AddDamage(50); // actual damage
            // }
            
            if (timeFalling > 1.2f)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.heavyLandSound, transform.position);
            }
            else if (timeFalling >0.15f)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.landSound, transform.position);
            }
            print("stop fall sound");
            fallInstance.stop(STOP_MODE.IMMEDIATE);
            CoreManager.Instance.AudioManager.RemoveSoundToStopWhenGoingToMainMenu(fallInstance);

            fallInstance.release();
            

            isFalling = false;
            timeFalling = 0f;
        }
    }

    private void CheckIfGrounded()
    {
        if (IsGrounded())
        {
            _canDash = true;
            if (isJumping)
            {
                isJumping = false;
                _rb.gravityScale = regularGravity;
            }

            if (isWallJumping && !hasWallJumped)
            {
                isWallJumping = false;
                _rb.gravityScale = regularGravity;
            }

            hasWallJumped = false;
            _rb.gravityScale = regularGravity;
        }
    }

    private void Move()
    {
        if (player.IsKnockBacked) return;
        if (Mathf.Abs(_moveInputX) > 0.1)
        {
            playerSoundHandler.HandleGroundSound();
        }
        else
        {
            playerSoundHandler.StopGroundSound();
        }

        if (player.InputEnabled)
        {

            _rb.linearVelocity =
                new Vector2(
                    _moveInputX * MovementSpeed *
                    Time.fixedDeltaTime, Mathf.Max(_rb.linearVelocity.y, maxFallingSpeed));
        }
        else
        {
            _rb.linearVelocity =
                new Vector2(
                    0, Mathf.Max(_rb.linearVelocity.y, maxFallingSpeed -2)); // we are falling, we let falling be faster
        }
    }

    public void HandleMovment(InputAction.CallbackContext context)
    {
        lastMoveTime = Time.time;
        if (player.InputEnabled == false)
        {
            print("enput is disabled, make entereted input true");
            enteredInput = true; // used for the start of the game, wakiping the player up
            return;
        }
            
        _moveInput = context.ReadValue<Vector2>();
        _moveInputX = _moveInput.x;
        _moveInputY = _moveInput.y;
        

        if (_moveInputX != 0)
        {
            HandleFlip(_moveInputX > 0);
        }
    }

    public void HandleDash(InputAction.CallbackContext context)
    {
        if (!player.InputEnabled || !enableDash) return;

        bool isBlockedWallSlide = isWallSliding && Mathf.Abs(_moveInputX) > 0.1f && _rb.linearVelocity.y < -1f;
        if (isBlockedWallSlide) return;

        if (context.started && CanDash())
        {
            Vector2 dir = _isFacingRight ? Vector2.right : Vector2.left;

            if (_moveInput != Vector2.zero)
            {
                if (enableAdvancedDash)
                {
                    dir = _moveInput;
                }
                else if (_moveInput.x != 0)
                {
                    dir = new Vector2(_moveInput.x, 0f);
                }
            }

            _lastDashDir = dir.normalized;
            _isDashing = true;
            isJumping = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
        }
    }


    private bool CanDash()
    {
        if (Time.time - LastPressedDashTime >= dashCoolDownTime && !_isDashing && _canDash)
        {
            LastPressedDashTime = Time.time;
            return true;
        }

        return false;
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (player.InputEnabled == false) return;

        if (context.started)
        {
            if (Time.time - LastPressedJumpTime < minJumpInterval) return;
            if (enableWallJump && isTouchingWall && !IsGrounded())
            {
                CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.jumpSound, transform.position);

                StartWallJump();
            }
            else if (CanJump() && !isWallJumping)
            {
                CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.jumpSound, transform.position);

                StartJumping();
            }
        }
        else if (context.canceled)
        {
            jumpIsPressed = false;
            _rb.gravityScale = WhenStopPressGravity;
        }
    }

    private void StartJumping()
    {
        LastPressedJumpTime = Time.time;
        jumpIsPressed = true;
        jumpTimer = StartCoroutine(JumpTimeout(0.4f));
        _rb.gravityScale = regularGravity;
        jumpFeedback?.PlayFeedbacks();
        Jump();
    }

    private IEnumerator JumpTimeout(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (jumpIsPressed)
        {
            jumpIsPressed = false;
            _rb.gravityScale = WhenStopPressGravity;
        }
    }

    private void HandleFlip(bool isMovingRight)
    {
        if (player.InputEnabled == false) return;
        if (isMovingRight != _isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (_isFacingRight)
        {
            var rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
        else
        {
            var rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

        _cameraFollowObject.CallTurn();

        _isFacingRight = !_isFacingRight;
    }
    
    public void FlipSpecial() // edge caes with wall sliding and dying at teh same time
    {
        if (_isFacingRight)
        {
            var rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }
        else
        {
            var rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
        }

        _cameraFollowObject.CallTurn();

        _isFacingRight = !_isFacingRight;
    }

    private bool IsGrounded()
    {
        foreach (LayerMask layer in groundLayers)
        {
            if (Physics2D.OverlapBox(groundCheckPosition.position, checkSize, 0, layer) != null)
            {
                LastOnGroundTime = 0f;
                return true;
            }
        }

        return false;
    }

    private bool IsTouchingWall()
    {
        return Physics2D.OverlapBox(wallCheckPosition.position, wallCheckSize, 0, wallLayer) != null;
    }

    private bool IsHittingSomething(Vector2 dir)
    {
        dir.Normalize();
        // Check ceiling if moving diagonally upward (both x and y are significant)
        if (dir.y > 0.45f)
        {
            Debug.DrawRay(cielingCheckPos.position, Vector2.up * 0.1f, Color.green);
            RaycastHit2D ceilingHit = Physics2D.Raycast(cielingCheckPos.position, Vector2.up, 0.1f);

            if (ceilingHit.collider != null &&
                ceilingHit.collider.gameObject != gameObject &&
                ceilingHit.collider.gameObject.layer != LayerMask.NameToLayer("Trigger"))
            {
                Debug.Log($"Hit ceiling: {ceilingHit.collider.name}");
                
                CheckForBreakable(ceilingHit.collider, dir);
                return true;
            }
        }
        else if (Mathf.Abs(dir.x) > 0.3f && Mathf.Abs(dir.y )< 0.75)
        {
            Collider2D wallHit = Physics2D.OverlapBox(wallCheckPosition.position, wallCheckSize, 0f,
                LayerMask.GetMask("Ground", "Default", "Enemy", "Environment", "WoodPlank"));

            if (IsValidHit(wallHit))
            {
                Debug.Log("Hit wall!");
                CheckForBreakable(wallHit, dir);
                return true;
            }
        }
        else
        {
            Collider2D[] groundHits = Physics2D.OverlapBoxAll(transform.position -Vector3.down*0.2f, wallCheckSize, 0f,
                LayerMask.GetMask("Ground", "Default", "Enemy", "Environment", "WoodPlank"));
            
            StartCoroutine(DrawOverlapBoxForSeconds(groundCheckPosition.position, wallCheckSize, Color.green, 3f));
            foreach (var hit in groundHits)
            {
                if (IsValidHit(hit))
                {
                    Debug.Log($"Hit ground with {hit.name}");
                    CheckForBreakable(hit, dir);
                    return true;
                }
            }
          
        }
     

        return false;
    }

    private IEnumerator DrawOverlapBoxForSeconds(Vector2 center, Vector2 size, Color color, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 halfSize = size * 0.5f;

            Vector2 topLeft = center + new Vector2(-halfSize.x, halfSize.y);
            Vector2 topRight = center + new Vector2(halfSize.x, halfSize.y);
            Vector2 bottomRight = center + new Vector2(halfSize.x, -halfSize.y);
            Vector2 bottomLeft = center + new Vector2(-halfSize.x, -halfSize.y);

            Debug.DrawLine(topLeft, topRight, color);
            Debug.DrawLine(topRight, bottomRight, color);
            Debug.DrawLine(bottomRight, bottomLeft, color);
            Debug.DrawLine(bottomLeft, topLeft, color);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    private bool IsValidHit(Collider2D collider)
    {
        if (collider)
        {
            print($"hit {collider.name} ");
  
        }
        return collider != null && collider.gameObject != gameObject;
    }

    private void CheckForBreakable(Collider2D collider, Vector2 dir)
    {
        var breakable = collider.GetComponent<IBreakable>();
        print($"hit {collider.name}");
        if (breakable != null)
        {
            print("breakable found");
            breakable.OnHit(dir, player.playerStage);
        }
    }


    private void WallSlide()
    {
        if (player.playerStage == PlayerStage.Young || player.playerStage == PlayerStage.Teen || player.playerStage == PlayerStage.Adult) return;
        if (isTouchingWall && !IsGrounded() && !isWallJumping &&
            ((_isFacingRight && _moveInputX > 0) || (!_isFacingRight && _moveInputX < 0)))
        {
            if (_rb.linearVelocity.y < 0)
            {
                isWallSliding = true;
                
                _rb.linearVelocity = new Vector2(0, Mathf.Clamp(_rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
            }
            else
            {
                isWallSliding = false;
                _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            }
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, checkSize);
        Gizmos.color = Color.blue;
        if (wallCheckPosition != null)
            Gizmos.DrawWireCube(wallCheckPosition.position, wallCheckSize);
    }

    private void Jump()
    {
        spineControl.PlayAnimation("jumpv2", false);
        _playedStartJump = true;
        float force = jumpForce;
        if (_rb.linearVelocity.y < 0)
            force -= _rb.linearVelocity.y;
        // CoreManager.Instance.AudioManager.PlayOneShot(co);
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x,
            _rb.linearVelocity.y + ((force + (0.5f * Time.fixedDeltaTime * -_rb.gravityScale)) / _rb.mass));
        LastOnGroundTime = 0f;
        isJumping = true;
    }

    private void StartWallJump()
    {
        CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.wallJumpSound, transform.position);
        isWallJumping = true;
        wallJumpCounter = wallJumpTime;
        wallJumpDirection = _isFacingRight ? -1 : 1;
        _rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpForceX, wallJumpForceY);
        _rb.gravityScale = wallJumpGravity;
        hasWallJumped = true;
    }

    private bool CanJump()
    {
        if (Time.time - LastPressedJumpTime < minJumpInterval) return false;
        return IsGrounded() || LastOnGroundTime <= graceJumpTime;
    }

    private void UpdateAnimation()
    {
        if (_preventAnimOverride) return;

        bool isCurrentlyGrounded = IsGrounded();

        if (isWallSliding && !_isPlayingWallJumpLand)
        {
            spineControl.PlayAnimation("wallslide", true);
        }
        else if (isWallJumping && wallJumpCounter > wallJumpTime - 0.1f)
        {
            spineControl.PlayAnimation("walljump-jump", false, "", true);
        }
        else if (hasWallJumped && isCurrentlyGrounded)
        {
            hasWallJumped = false;
            _isPlayingWallJumpLand = true;

            spineControl.PlayAnimation("walljump-land-turn", false, "", true,
                () => { _isPlayingWallJumpLand = false; });
        }

        else if (!_wasGroundedLastFrame && isCurrentlyGrounded)
        {
            landFeedback?.PlayFeedbacks();
            if (makeNextfallHard)
            {
                spineControl.PlayAnimation("fall-hard",3, false, "", true);
                _playedStartJump = false;
                makeNextfallHard = false;
                return;
            }
            spineControl.PlayAnimation("jump-land", false, "", true);
            _playedStartJump = false;
        }
        else if (_rb.linearVelocity.y < -0.2f && !isCurrentlyGrounded && !_isDashing && !isWallJumping &&
                 !isWallSliding)
        {
            spineControl.PlayAnimation("jump-air", true);
        }
        else if (!_isDashing && !_preventAnimOverride && !_playedStartJump)
        {
            if (isCurrentlyGrounded && Mathf.Abs(_moveInputX) > 0.2f)
            {
                if (isWalkingScene)
                {

                    spineControl.PlayAnimation("walkv2", true);
                }
                else
                {
                    spineControl.PlayAnimation("run", true);
                }
            }
            else if (isCurrentlyGrounded)
                spineControl.PlayAnimation("idlev2", true);
        }

        _wasGroundedLastFrame = isCurrentlyGrounded;
    }

    private IEnumerator PreventOverrideForSeconds(float seconds)
    {
        _preventAnimOverride = true;
        yield return new WaitForSeconds(seconds);
        _preventAnimOverride = false;
    }


    private IEnumerator StartDash(Vector2 dir)
    {
        dashFeedback?.PlayFeedbacks();
        spineControl.ClearActionAnimation(); // Cancel jump/land/start-jump
        spineControl.PlayAnimation("dashv2", false);
        CoreManager.Instance.AudioManager.PlayOneShot(playerSounds.dashSound, transform.position);
        // this.StopAndStartCoroutine(ref dashHitCoroutine, CheckHitsWhileDashing());
        _isDashing = true;
        _canDash = false;
        player.SetForce();
        float startTime = Time.time;
        _isDashAttacking = true;
        _rb.gravityScale = 0;

        while (Time.time - startTime <= dashAttackTime && _isDashing)
        {
            if (!hitSomethingDuringDash && HornDamageManager.Instance.allowHornDamage && IsHittingSomething(dir))
            {
                print("called is hitting something");
                hitSomethingDuringDash = true;
                if (!player.IsGodMode)
                {
                    hornDamageHandler.AddDamage(1);
                }
            }

            if (player.IsKnockBacked)
            {
                _isDashAttacking = false;
                _rb.gravityScale = WhenStopPressGravity;
                player.ResetForce();
                _isDashing = false;
                print("reset dash from knockback");
                hitSomethingDuringDash = false;
                yield break;
            }

            _rb.linearVelocity = dir.normalized * dashSpeed;

            yield return null;
        }

        startTime = Time.time;
        _isDashAttacking = false;
        _rb.gravityScale = WhenStopPressGravity;
        _rb.linearVelocity = dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= dashEndTime)
        {
            yield return null;
        }
        print("reset dash");
        player.ResetForce();
        _isDashing = false;
        hitSomethingDuringDash = false;
    }
    // private IEnumerator CheckHitsWhileDashing()
    // {
    //     CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
    //     if (capsule == null)
    //     {
    //         Debug.LogWarning("No CapsuleCollider2D found on player.");
    //         yield break;
    //     }
    //
    //     while (_isDashing)
    //     {
    //         // Perform a capsule cast in the dash direction
    //         RaycastHit2D hit = Physics2D.CapsuleCast(
    //             capsule.bounds.center,
    //             capsule.size,
    //             capsule.direction,
    //             0f, // no rotation
    //             DashDirection.normalized,
    //             0.1f // small distance
    //         );
    //
    //         if (hit.collider != null)
    //         {
    //             IBreakable breakable = hit.collider.GetComponent<IBreakable>();
    //             if (breakable != null)
    //             {
    //                 breakable.OnHit(DashDirection, player.playerStage);
    //             }
    //         }
    //
    //         yield return null;
    //     }
    public void ForcePlayerFlip()
    {
       
    }


    public void ResetToInitialState()
    {
        if (!isWallSliding) return;
        isWallSliding = false;
        Skeleton skeleton = spineControl.skeletonAnimation.Skeleton;
        skeleton.RootBone.Rotation = 0;
        skeleton.RootBone.ScaleX = 1;
        skeleton.RootBone.ScaleY = 1;
        skeleton.UpdateWorldTransform(Skeleton.Physics.None);
        // var rotator = new Vector3(transform.rotation.x, Mathf.Approximately(transform.rotation.y, 0) ? 180 : 0  , transform.rotation.z);
        // finalFormSpine.transform.rotation = Quaternion.Euler(rotator);
     
    }
}