using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PixelAdventure.Managers;
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private bool enableDoubleJump = true;
    [SerializeField] private float doubleJumpForce = 13f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private GameManager gameManager;
    private AudioManager audioManager;
    private float targetHorizontal = 0f;
    private bool jumpRequested = false;
    private bool isUIActive = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
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
    void OnEnable()
{
    // Tìm các nút UI trong scene
    Button jumpButton = GameObject.Find("Jump")?.GetComponent<Button>();
    EventTrigger leftButton = GameObject.Find("Left")?.GetComponent<EventTrigger>();
    EventTrigger rightButton = GameObject.Find("Right")?.GetComponent<EventTrigger>();
    GameObject jumpGO  = GameObject.Find("Jump"); 

    // Nút Jump
    if (jumpGO != null)
    {
        var jumpET = jumpGO.GetComponent<EventTrigger>();
        if (jumpET == null) jumpET = jumpGO.AddComponent<EventTrigger>();
        AddEventTrigger(jumpET, EventTriggerType.PointerDown, () => {
            // Gọi trực tiếp để nhảy ngay lập tức
            Jump();
        });
    }

    // Nút Left
    if (leftButton != null)
    {
        AddEventTrigger(leftButton, EventTriggerType.PointerDown, StartMoveLeft);
        AddEventTrigger(leftButton, EventTriggerType.PointerUp, StopMove);
    }

    // Nút Right
    if (rightButton != null)
    {
        AddEventTrigger(rightButton, EventTriggerType.PointerDown, StartMoveRight);
        AddEventTrigger(rightButton, EventTriggerType.PointerUp, StopMove);
    }
}

void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
{
    EventTrigger.Entry entry = new EventTrigger.Entry();
    entry.eventID = type;
    entry.callback.AddListener((eventData) => { action(); });
    trigger.triggers.Add(entry);
}
    void Update()
    {   
        if(gameManager.GetIsGameOver()||gameManager.GetIsGameWin())
        {
            return;
        }
        HandleInput();
        CheckGrounded();
        UpdateAnimation ();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        // Nếu UI không active, dùng bàn phím cho di chuyển
        if (!isUIActive)
        {
            float keyboardHorizontal = Input.GetAxisRaw("Horizontal");
            targetHorizontal = keyboardHorizontal;
        }

        // Xử lý nhảy từ bàn phím (luôn ưu tiên, không phụ thuộc UI)
        bool jumpPressed = Input.GetButtonDown("Jump") ||
                          Input.GetKeyDown(KeyCode.Space) ||
                          Input.GetKeyDown(KeyCode.W) ||
                          Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpPressed)
        {
            jumpRequested = true;
        }

        // Xử lý nhảy từ UI hoặc bàn phím
        if (jumpRequested)
        {
            PerformJump();
            jumpRequested = false;
        }
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

    private void PerformJump()
    {
        if (isGrounded)
        {
            // First jump (ground jump)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioManager.PlayJumpEffect();
            
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

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(targetHorizontal * speed, rb.linearVelocity.y);

        // Flip character based on movement direction
        if (targetHorizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (targetHorizontal < 0)
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
            bool isRunning = Mathf.Abs(targetHorizontal) > 0.1f && isGrounded;
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

    public void StartMoveLeft() { isUIActive = true; targetHorizontal = -1f; }
    public void StartMoveRight() { isUIActive = true; targetHorizontal = 1f; }
    public void StopMove() { isUIActive = false; targetHorizontal = 0f; }
    public void Jump() { jumpRequested = true; }
}
