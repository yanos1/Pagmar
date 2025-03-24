using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float MovementSpeed = 200f;
    [SerializeField] private float regularGravity = 1.2f;
    [SerializeField] private float WhenStopPressGravity = 2.5f;
    [SerializeField] private float whenJumpFallingGravity = 1.8f;
    [SerializeField] private float maxFallingSpeed = -10f;
    [SerializeField] private float HangGravity = 1f;
    [SerializeField] private float HangThreshold = 2f;
    
    private bool _isFacingRight = true;
    private float _moveInputX;
    private float _moveInputY;
    private bool IsJumpFalling = false;
    private Coroutine jumpTimer;
    public float LastPressedJumpTime = 0f;
    public float LastOnGroundTime = 0f;
    
    private bool isJumping = false;
    public bool jumpIsPressed = false;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPosition;
    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private LayerMask groundLayer;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        LastOnGroundTime = Time.time;
    }
    
    private void Update()
    {
        LastOnGroundTime+=Time.deltaTime;

        Move();
        if((isJumping|IsJumpFalling)&&Mathf.Abs(_rb.linearVelocity.y)<HangThreshold)
        {
            _rb.gravityScale = HangGravity;
            Debug.Log("Hang");
        }
        if(_rb.linearVelocity.y<-HangThreshold && !IsGrounded())
        {
            JumpFall();
        }
        
        if (IsGrounded() && IsJumpFalling)
        {
            IsJumpFalling = false;
            isJumping = false;
            _rb.gravityScale = regularGravity;
        }
    }
    private void Move()
    {
        _rb.linearVelocity = new Vector2(_moveInputX * MovementSpeed * Time.fixedDeltaTime,Mathf.Max(_rb.linearVelocity.y,maxFallingSpeed));
    }
    
    private void Start()
    {
        _isFacingRight = true;
    }
    
    // Called from your InputAction for movement
    public void HandleMovment(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _moveInputX = moveInput.x;
        _moveInputY = moveInput.y;
        
        if (_moveInputX != 0)
        {
            HandleFlip(_moveInputX > 0);
        }
    }
    
    // Called from your InputAction for jump
    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpIsPressed = true;
            if(CanJump())StartJumping();
        }
        else if (context.canceled)
        {
            jumpIsPressed = false;
            _rb.gravityScale = WhenStopPressGravity;
        }
    }
    private void StartJumping()
    {
        jumpTimer = StartCoroutine(JumpTimeout(0.4f));
        Jump();
        
    }
    private void JumpFall()
    {
        IsJumpFalling = true;
        _rb.gravityScale = whenJumpFallingGravity;
        Debug.Log("JumpFall");
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
        return Physics2D.OverlapBox(groundCheckPosition.position, checkSize, 0, groundLayer) != null;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPosition.position, checkSize);
    }
    
    private void Jump()
    {
        isJumping = true;
        LastPressedJumpTime = 0f;
        LastOnGroundTime = 0f;
        
        float force = jumpForce;
        if (_rb.linearVelocity.y < 0)
            force -= _rb.linearVelocity.y;
        
        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }
    
    private bool CanJump()
    {
        return IsGrounded() && !isJumping && LastOnGroundTime > 0.1f;
    }
}
