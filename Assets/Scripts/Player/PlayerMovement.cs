using System.Collections;
using SpongeScene;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f; // Base move speed
        [SerializeField] private float jumpHeight = 5f; // Jump height
        [SerializeField] private float dashSpeed = 15f; // Second dash speed
        [SerializeField] private float dashTime = 0.4f; // Total dash time (two dashes with a delay)
        [SerializeField] private float maxSpeed = 10f; // Maximum speed
        [SerializeField] private float verticalRamAmount = 1.5f; // Vertical movement during the second dash (up and down movement)
        [SerializeField] private float rotationAmount = 30f; // The amount of rotation during the dash (degrees)
        [SerializeField] private float rotationDuration = 0.3f;
    
        private float currentSpeed = 0f; // Current movement speed
        private bool isDashing = false; // If the player is currently dashing
        private bool isGrounded = true; // Check if the player is grounded
        private bool isFacingRight = true; // Check if the player is facing right

        private Rigidbody2D rb; // Reference to the player's Rigidbody2D
        private Vector2 moveInput;

        public bool IsDashing => isDashing;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        }

        void Update()
        {
            HandleInput();
            HandleJump();
            CheckIfGrounded();
        }

        private void CheckIfGrounded()
        {
            if (Physics2D.Raycast(transform.position, Vector2.down, 0.3f))
            {
                isGrounded = true;
            }
        }

        void FixedUpdate()
        {
            if (!isDashing)
            {
                MovePlayer();
            }
        }

        void HandleInput()
        {
            // Getting horizontal movement input
            moveInput = new Vector2(Input.GetAxis("Horizontal"), 0);

            // Trigger dash only when LeftShift is pressed, if not already dashing
            if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Mathf.Abs(moveInput.x) > 0)
            {
                StartCoroutine(Dash());
            }
        }

        void HandleJump()
        {
            // Jumping if the player is grounded and presses the jump key (e.g., space)
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
                isGrounded = false;
            }
        }

        void MovePlayer()
        {
            // Increase speed gradually while moving, capped at maxSpeed
            if (moveInput.x != 0)
            {
                currentSpeed += moveSpeed * Time.deltaTime * 5;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
            }
            else
            {
                currentSpeed = 0f;
            }

            // Apply movement to the Rigidbody2D
            rb.linearVelocity = new Vector2(moveInput.x * currentSpeed, rb.linearVelocity.y);

            // Flip the player to face the direction they're moving in
            if (moveInput.x > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput.x < 0 && isFacingRight)
            {
                Flip();
            }
        }

        IEnumerator Dash()
        {
            isDashing = true;
            float initialDirection = moveInput.x > 0 ? 1 : -1;
        
            yield return StartCoroutine(UtilityFunctions.RotateOverTime(transform, rotationAmount * -initialDirection, rotationDuration));
            rb.linearVelocity = new Vector2(initialDirection * dashSpeed, verticalRamAmount); // Move vertically and horizontally

            yield return new WaitForSeconds(0.7f); // Wait until the dash is done
            yield return StartCoroutine(UtilityFunctions.RotateOverTime(transform, -rotationAmount * -initialDirection, rotationDuration/2));

            // End dashing after the second dash
            isDashing = false;


            // Reset rotation to normal after dash
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }


        // Flip the player to face the other direction
        void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 playerScale = transform.localScale;
            playerScale.x *= -1; // Flip horizontally
            transform.localScale = playerScale;
        }
    }
}
