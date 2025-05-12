using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MovementSpeed = 200f;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float regularGravity = 1.2f;
    [SerializeField] private float WhenStopPressGravity = 2.5f;
    [SerializeField] private float maxFallingSpeed = -10f;
    [SerializeField] private float graceJumpTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private List<LayerMask> groundLayers;

    [Header("Wall Jump & Slide")]
    [SerializeField] private Transform wallCheckPosition;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.5f, 1f);
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpForceX = 10f;
    [SerializeField] private float wallJumpForceY = 12f;
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float wallJumpGravity = 0.8f;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 150f;
    [SerializeField] private float dashAttackTime = 0.15f;
    [SerializeField] private float dashEndTime = 0.15f;
    [SerializeField] private float dashEndSpeed = 50f;
    [SerializeField] private float dashCoolDownTime = 1f;

    [Header("CammeraFollowObject")]
    [SerializeField] private CameraFollowObject _cameraFollowObject;

    private bool isJumping = false;
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
    public bool IsFacingRight => _isFacingRight;
    public bool IsDashing => _isDashing;
    public Vector2 DashDirection => _lastDashDir;
    public Vector3 GroundCheckPos => groundCheckPosition.position;

    private bool isTouchingWall = false;
    private bool isWallJumping = false;
    private float wallJumpDirection;
    private float wallJumpCounter;
    private bool hasWallJumped = false;
    private bool isWallSliding = false;
    
    [SerializeField] private bool enableWallJump = true;
    private PlayerManager player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        LastOnGroundTime = Time.time;
        LastPressedDashTime = Time.time;
        _isFacingRight = true;
        player = GetComponent<PlayerManager>();
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

        if (!_isDashAttacking && !isWallJumping && !isWallSliding && player.InputEnabled)
        {
            print($"input enabled: {player.InputEnabled} so i can move!");
            Move();
        }

        CheckIfGrounded();
        CheckIfFalling();

        isTouchingWall = IsTouchingWall();
        // Disable dash when touching a wall
        if (isTouchingWall) _canDash = false;

        if (isWallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter <= 0 && _rb.linearVelocity.y > 0)
            {
                isWallJumping = false;
                _rb.gravityScale = regularGravity;
            }
        }

        WallSlide();
    }

    private void CheckIfFalling()
    {
        if (_rb.linearVelocity.y < _fallSpeedYDampingChangeThreshold &&
            !CameraManager.GetInstance().IsLerpingYDamping && !CameraManager.GetInstance().LerpedFromPlayerFalling)
        {
            CameraManager.GetInstance().LerpYDamping(true);
        }

        if (_rb.linearVelocity.y >= 0 &&
            !CameraManager.GetInstance().IsLerpingYDamping && CameraManager.GetInstance().LerpedFromPlayerFalling)
        {
            CameraManager.GetInstance().LerpedFromPlayerFalling = false;
            CameraManager.GetInstance().LerpYDamping(false);
        }
    }

    private void CheckIfGrounded()
    {
        if (IsGrounded())
        {
            _canDash = true;
            if(isJumping)
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
        if (player.IsInjured)
        {
            _rb.linearVelocity = new Vector2(_moveInputX * MovementSpeed*0.5f * Time.fixedDeltaTime, Mathf.Max(_rb.linearVelocity.y, maxFallingSpeed));
            print("moving injured 78");
        }
        _rb.linearVelocity = new Vector2(_moveInputX * MovementSpeed * Time.fixedDeltaTime, Mathf.Max(_rb.linearVelocity.y, maxFallingSpeed));
    }

    public void HandleMovment(InputAction.CallbackContext context)
    {
        if(player.InputEnabled == false) return;
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
        if(player.InputEnabled == false) return;

        if (context.started && CanDash())
        {
            if (_moveInput != Vector2.zero)
                _lastDashDir = _moveInput;
            else
                _lastDashDir = _isFacingRight ? Vector2.right : Vector2.left;

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
        if(player.InputEnabled == false) return;

        if (context.started)
        {
            if (enableWallJump && isTouchingWall && !IsGrounded())
            {
                StartWallJump();
            }
            else if (CanJump() && !isWallJumping)
            {
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
        LastPressedJumpTime = 0f;
        jumpIsPressed = true;
        jumpTimer = StartCoroutine(JumpTimeout(0.4f));
        _rb.gravityScale = regularGravity;
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
        if(player.InputEnabled == false) return;
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
            _cameraFollowObject.CallTurn();
        }
        else
        {
            var rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _cameraFollowObject.CallTurn();
        }
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

    private void WallSlide()
    {
        if (isTouchingWall && !IsGrounded() && !isWallJumping &&((_isFacingRight && _moveInputX >0) || (!_isFacingRight && _moveInputX< 0)))
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
        float force = jumpForce;
        if (player.IsInjured)
        {
            force = jumpForce * 0.8f;
        }
        if (_rb.linearVelocity.y < 0)
            force -= _rb.linearVelocity.y;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y + ((force + (0.5f * Time.fixedDeltaTime * -_rb.gravityScale)) / _rb.mass));
        LastOnGroundTime = 0f;
        isJumping = true;
    }

    private void StartWallJump()
    {
        isWallJumping = true;
        wallJumpCounter = wallJumpTime;
        wallJumpDirection = _isFacingRight ? -1 : 1;
        _rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpForceX, wallJumpForceY);
        _rb.gravityScale = wallJumpGravity;
        hasWallJumped = true;
    }

    private bool CanJump()
    {
        return IsGrounded() || LastOnGroundTime <= graceJumpTime;
    }

    private IEnumerator StartDash(Vector2 dir)
    {
        _isDashing = true;
        _canDash = false;
        player.SetForce();
        float startTime = Time.time;
        _isDashAttacking = true;
        _rb.gravityScale = 0;

        while (Time.time - startTime <= dashAttackTime)
        {
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
        player.ResetForce();
        _isDashing = false;
    }
}
