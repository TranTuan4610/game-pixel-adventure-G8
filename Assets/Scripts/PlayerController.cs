using UnityEngine;
using PixelAdventure.Managers;
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private bool enableDoubleJump = true;
    [SerializeField] private float doubleJumpForce = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private GameManager gameManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();

        // Auto-create ground check if not assigned
        if (groundCheck == null)
        {
            CreateGroundCheck();
        }
    }

    private void CreateGroundCheck()
    {
        // Create ground check GameObject
        GameObject groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.SetParent(transform);

        // Position it at the bottom of the player
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            float bottomY = playerCollider.bounds.min.y - transform.position.y - 0.1f;
            groundCheckObj.transform.localPosition = new Vector3(0, bottomY, 0);
        }
        else
        {
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        }

        // Assign to groundCheck field
        groundCheck = groundCheckObj.transform;

    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Auto-setup layer mask if not set
        if (groundLayerMask.value == 0)
        {
            SetupDefaultLayerMask();
        }
    }

    private void SetupDefaultLayerMask()
    {
        // Try to find "Ground" layer
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            groundLayerMask = 1 << groundLayer;

        }
        else
        {
            // Use Default layer as fallback
            groundLayerMask = 1 << LayerMask.NameToLayer("Default");
         
        }
    }

    void Update()
    {   
        if(gameManager.GetIsGameOver()||gameManager.GetIsGameWin())
        {
            return;
        }
        HandleInput();
        CheckGrounded();
        HandleJump();
        UpdateAnimation ();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        // Get horizontal input for movement
    }

    private void CheckGrounded()
    {
        Vector3 checkPosition;
        float checkRadius;

        if (groundCheck != null)
        {
            // Use assigned ground check transform
            checkPosition = groundCheck.position;
            checkRadius = groundCheckRadius;
        }
        else
        {
            // Fallback: use player's bottom position
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                checkPosition = new Vector3(transform.position.x, playerCollider.bounds.min.y - 0.1f, transform.position.z);
            }
            else
            {
                checkPosition = transform.position + Vector3.down * 0.6f;
            }
            checkRadius = 0.2f;
        }

        // Perform ground check
        if (groundLayerMask.value == 0)
        {
            // If no layer mask set, check against all layers
            isGrounded = Physics2D.OverlapCircle(checkPosition, checkRadius) != null;
        }
        else
        {
            isGrounded = Physics2D.OverlapCircle(checkPosition, checkRadius, groundLayerMask) != null;
        }

        // Reset double jump when grounded
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }
    }

    private void HandleJump()
    {
        // Check multiple jump inputs for easier testing
        bool jumpPressed = Input.GetButtonDown("Jump") ||
                          Input.GetKeyDown(KeyCode.Space) ||
                          Input.GetKeyDown(KeyCode.W) ||
                          Input.GetKeyDown(KeyCode.UpArrow);

        if (jumpPressed)
        {
            if (isGrounded)
            {
                // First jump (ground jump)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                    animator.SetInteger("jumpCount", 1);
                    animator.SetBool("isFalling", false); // Ensure we're not in falling state
                }
            }
            else if (enableDoubleJump && !hasDoubleJumped)
            {
                // Second jump (double jump)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
                hasDoubleJumped = true;

                if (animator != null)
                {
                    // Interrupt fall animation with jump
                    animator.SetBool("isFalling", false);
                    animator.SetTrigger("Jump");
                    animator.SetInteger("jumpCount", 2);
                    // After jump animation completes, return to fall if still in air
                    StartCoroutine(ResetJumpCountAfterDelay(0.3f));
                }
            }
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        // Flip character based on movement direction
        if(horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if(horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw ground check radius in editor
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Basic state checks
            bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded;
            bool isJumping = !isGrounded && rb.linearVelocity.y > 0;
            bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;
            
            int currentJumpCount = animator.GetInteger("jumpCount");
            
            // Handle animation states
            if (currentJumpCount > 0)
            {
                // In jump animation - prioritize over fall
                isFalling = false;
            }
            else if (isFalling && !isJumping)
            {
                // Show fall animation when not jumping
                animator.SetBool("isFalling", true);
            }

            // Set animation parameters
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isJumping", isJumping);
            animator.SetBool("isFalling", isFalling);

            // Reset states only when grounded
            if (isGrounded)
            {
                animator.SetInteger("jumpCount", 0);
                animator.SetBool("isFalling", false);
            }

            // Note: Animation logic:
            // - PlayerJump1: Normal jump (jumpCount = 1) - Priority HIGH
            // - PlayerJump: Double jump (jumpCount = 2) - Priority HIGH
            // - PlayerFall: Falling down (isFalling = true, jumpCount = 0) - Priority LOW
            // - PlayerIdle: Grounded and not moving
            // - PlayerRun: Grounded and moving
        }
    }

    // Reset jump count after animation plays to allow fall animation
    private System.Collections.IEnumerator ResetJumpCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null && !isGrounded && rb.linearVelocity.y < 0)
        {
            animator.SetInteger("jumpCount", 0);
            animator.SetBool("isFalling", true);  // Re-enable falling animation
        }
    }

    // Public API used by LevelManager to reset player state at spawn
    public void ResetPlayer()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Stop all movement
        rb.linearVelocity = Vector2.zero;

        // Re-enable control
        enabled = true;

        // Clear animation state if available
        if (animator != null)
        {
            animator.ResetTrigger("Jump");
            animator.SetInteger("jumpCount", 0);
            animator.SetBool("isFalling", false);
        }
    }

    // Animation Event Functions (called by Animation Events if they exist)
    public void OnLandingAnimationEvent()
    {
        // Called when landing animation plays
    }

    public void OnJumpAnimationEvent()
    {
        // Called when jump animation plays
    }

    public void OnFootstepAnimationEvent()
    {
        // Called during run animation for footstep sounds
    }
}
