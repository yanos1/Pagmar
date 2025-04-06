using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
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
    
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPosition;
        [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 0.1f);
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask enemyLayer;
    
        [Header("Dash")]
        [SerializeField] private float dashSpeed = 150f;
        [SerializeField] private float dashAttackTime = 0.15f;
        [SerializeField] private float dashEndTime = 0.15f;
        [SerializeField] private float dashEndSpeed = 50f;
        [SerializeField] private float dashCoolDownTime = 1f;
    
    
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

        public bool IsDashing => _isDashing;

    
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            LastOnGroundTime = Time.time;
            LastPressedDashTime = Time.time;
        
        }
    
        private void Update()
        {
            LastOnGroundTime+=Time.deltaTime;

            Move();
            CheckIfGrounded();
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
            }
        }
        private void Move()
        {
            if(_isDashAttacking) return;
            _rb.linearVelocity = new Vector2(_moveInputX * MovementSpeed * Time.fixedDeltaTime,Mathf.Max(_rb.linearVelocity.y,maxFallingSpeed));
        }
    
        private void Start()
        {
            _isFacingRight = true;
        }
    
        public void HandleMovment(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            print($"move input is {_moveInput}");
            _moveInputX = _moveInput.x;
            _moveInputY = _moveInput.y;
        
            if (_moveInputX != 0)
            {
                HandleFlip(_moveInputX > 0);
            }
        }
    
        public void HandleDash(InputAction.CallbackContext context)
        {
            if (context.started && CanDash())
            {
                Debug.Log("Dash");
                if (_moveInput != Vector2.zero)
                    _lastDashDir = _moveInput;
                else
                    _lastDashDir = _isFacingRight ? Vector2.right : Vector2.left;

                _isDashing = true;
                isJumping = false;
                // IsWallJumping = false;
                // _isJumpCut = false;

                StartCoroutine(nameof(StartDash), _lastDashDir);
            }
        
        }
        private bool CanDash()
        {
            if (Time.time - LastPressedDashTime >= dashCoolDownTime && !_isDashing&&_canDash)
            {
                LastPressedDashTime = Time.time;
                return true;
            }
            return false;
        }
    
        // Called from your InputAction for jump
        public void HandleJump(InputAction.CallbackContext context)
        {
            if (context.started&&CanJump())
            {
                StartJumping();
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
            if (isMovingRight != _isFacingRight)
            {
                Flip();
            }
        }
    
        private void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            _isFacingRight = !_isFacingRight;
        }
    
        private bool IsGrounded()
        {
            if (Physics2D.OverlapBox(groundCheckPosition.position, checkSize, 0))
            {
                LastOnGroundTime = 0f;
                return true;
            }

            return false;
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheckPosition.position, checkSize);
        }
    
        private void Jump()
        {
            float force = jumpForce;
            if (_rb.linearVelocity.y < 0)
                force -= _rb.linearVelocity.y;
        
            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            LastOnGroundTime = 0f;
            isJumping = true;


        }
    
        private bool CanJump()
        {
            return IsGrounded();
        }
    
        private IEnumerator StartDash(Vector2 dir)
        {
            _isDashing = true;
            _canDash = false;

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
            _isDashing = false;

        }
    }
}
