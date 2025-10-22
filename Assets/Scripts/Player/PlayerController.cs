using UnityEngine;
using PixelAdventure.Managers;

namespace PixelAdventure.Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpForce = 12f;
        public float coyoteTime = 0.2f;
        public float jumpBufferTime = 0.2f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayerMask;

        [Header("Animation")]
        public Animator animator;

        // Components
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;

        // Movement variables
        private float horizontalInput;
        private bool isGrounded;
        private bool wasGrounded;
        private float coyoteTimeCounter;
        private float jumpBufferCounter;

        // Animation parameters
        private readonly int moveSpeedParam = Animator.StringToHash("MoveSpeed");
        private readonly int isGroundedParam = Animator.StringToHash("IsGrounded");
        private readonly int jumpParam = Animator.StringToHash("Jump");

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        private void Update()
        {
            HandleInput();
            CheckGrounded();
            HandleCoyoteTime();
            HandleJumpBuffer();
            HandleJump();
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void HandleInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
        }

        private void CheckGrounded()
        {
            wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

            // Reset jump buffer when landing
            if (!wasGrounded && isGrounded)
            {
                jumpBufferCounter = 0f;
            }
        }

        private void HandleCoyoteTime()
        {
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }
        }

        private void HandleJumpBuffer()
        {
            if (jumpBufferCounter > 0f)
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }

        private void HandleJump()
        {
            if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            {
                Jump();
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
            }
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            // Play jump animation
            if (animator != null)
            {
                animator.SetTrigger(jumpParam);
            }

            // Play jump sound
            AudioManager.Instance?.PlaySound("Jump");
        }

        private void HandleMovement()
        {
            // Apply horizontal movement
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

            // Flip sprite based on movement direction
            if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true;
            }
        }

        private void UpdateAnimations()
        {
            if (animator != null)
            {
                animator.SetFloat(moveSpeedParam, Mathf.Abs(horizontalInput));
                animator.SetBool(isGroundedParam, isGrounded);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Handle collectibles
            if (other.CompareTag("Collectible"))
            {
                CollectItem(other.gameObject);
            }
            // Handle level finish
            else if (other.CompareTag("Finish"))
            {
                CompleteLevel();
            }
            // Handle enemies or hazards
            else if (other.CompareTag("Enemy") || other.CompareTag("Hazard"))
            {
                TakeDamage();
            }
        }

        private void CollectItem(GameObject item)
        {
            // Add score
            FindObjectOfType<GameManager>()?.AddScore(100);
            
            // Play collect sound
            AudioManager.Instance?.PlaySound("Collect");
            
            // Destroy the item
            Destroy(item);
        }

        private void CompleteLevel()
        {
            // Disable player input
            enabled = false;
            
            // Complete level
            FindObjectOfType<GameManager>()?.CompleteLevel();
        }

        private void TakeDamage()
        {
            // Play damage sound
            AudioManager.Instance?.PlaySound("Damage");
            
            // Lose a life
            FindObjectOfType<GameManager>()?.LoseLife();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw ground check radius
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }

        // Public methods for external control
        public void SetInputEnabled(bool enabled)
        {
            this.enabled = enabled;
        }

        public void ResetPlayer()
        {
            rb.linearVelocity = Vector2.zero;
            enabled = true;
        }
    }
}
