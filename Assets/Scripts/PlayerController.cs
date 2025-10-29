using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
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

    // Tên các nút (đổi nếu khác trong Canvas)
    [Header("UI Button Names")]
    [SerializeField] private string leftButtonName = "Left";
    [SerializeField] private string rightButtonName = "Right";
    [SerializeField] private string jumpButtonName = "Jump";

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool hasDoubleJumped;
    private GameManager gameManager;
    private AudioManager audioManager;

    // Input
    private float targetHorizontal = 0f;
    private bool jumpRequested = false;

    // UI input flags
    private bool isUIActive = false;
    private bool leftHeld = false;
    private bool rightHeld = false;

    // tránh gắn listener nhiều lần
    private bool _wiredUI = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        if (groundCheck == null) CreateGroundCheck();
    }

    private void CreateGroundCheck()
    {
        var groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.SetParent(transform);

        var playerCol = GetComponent<Collider2D>();
        if (playerCol != null)
        {
            float bottomY = playerCol.bounds.min.y - transform.position.y - 0.1f;
            groundCheckObj.transform.localPosition = new Vector3(0, bottomY, 0);
        }
        else
        {
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        groundCheck = groundCheckObj.transform;
    }

    void Start()
    {
        if (groundLayerMask.value == 0)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            groundLayerMask = (groundLayer != -1) ? (1 << groundLayer) : (1 << LayerMask.NameToLayer("Default"));
        }

        // Cho phép đa chạm trên device
        Input.multiTouchEnabled = true;

        WireUIIfNeeded(); // gắn EventTrigger 1 lần
    }

    private void WireUIIfNeeded()
    {
        if (_wiredUI) return; // chỉ gắn 1 lần / process

        // Jump
        var jumpGO = GameObject.Find(jumpButtonName);
        if (jumpGO)
        {
            var et = jumpGO.GetComponent<EventTrigger>();
            if (et == null) et = jumpGO.AddComponent<EventTrigger>();
            if (et.triggers == null) et.triggers = new List<EventTrigger.Entry>();

            AddET(et, EventTriggerType.PointerDown, () =>
            {
                // Nhảy lập tức (không delay)
                Jump();
            });
        }

        // Left
        var leftGO = GameObject.Find(leftButtonName);
        if (leftGO)
        {
            var et = leftGO.GetComponent<EventTrigger>();
            if (et == null) et = leftGO.AddComponent<EventTrigger>();
            if (et.triggers == null) et.triggers = new List<EventTrigger.Entry>();

            AddET(et, EventTriggerType.PointerDown, OnLeftDown);
            AddET(et, EventTriggerType.PointerUp, OnLeftUp);
        }

        // Right
        var rightGO = GameObject.Find(rightButtonName);
        if (rightGO)
        {
            var et = rightGO.GetComponent<EventTrigger>();
            if (et == null) et = rightGO.AddComponent<EventTrigger>();
            if (et.triggers == null) et.triggers = new List<EventTrigger.Entry>();

            AddET(et, EventTriggerType.PointerDown, OnRightDown);
            AddET(et, EventTriggerType.PointerUp, OnRightUp);
        }

        _wiredUI = true;
    }

    private void AddET(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    private void Update()
    {
        if (gameManager.GetIsGameOver() || gameManager.GetIsGameWin()) return;

        HandleInput();      // Keyboard + UI flags
        CheckGrounded();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        // Bàn phím (Editor/PC)
        float kb = Input.GetAxisRaw("Horizontal");

        // Tính hướng từ UI
        float uiDir = 0f;
        if (leftHeld) uiDir -= 1f;
        if (rightHeld) uiDir += 1f;

        // Ưu tiên phím nếu có nhấn, ngược lại dùng UI
        if (Mathf.Abs(kb) > 0.01f)
        {
            targetHorizontal = kb;
            isUIActive = false;
        }
        else
        {
            targetHorizontal = uiDir;
            isUIActive = Mathf.Abs(uiDir) > 0.01f;
        }

        // Nhảy từ bàn phím
        bool jumpPressed = Input.GetButtonDown("Jump") ||
                           Input.GetKeyDown(KeyCode.Space) ||
                           Input.GetKeyDown(KeyCode.W) ||
                           Input.GetKeyDown(KeyCode.UpArrow);
        if (jumpPressed) Jump();
    }

    private void CheckGrounded()
    {
        Vector3 pos;
        float radius;

        if (groundCheck != null)
        {
            pos = groundCheck.position;
            radius = groundCheckRadius;
        }
        else
        {
            var col = GetComponent<Collider2D>();
            pos = (col != null)
                ? new Vector3(transform.position.x, col.bounds.min.y - 0.1f, transform.position.z)
                : transform.position + Vector3.down * 0.6f;
            radius = 0.2f;
        }

        isGrounded = (groundLayerMask.value == 0)
            ? Physics2D.OverlapCircle(pos, radius) != null
            : Physics2D.OverlapCircle(pos, radius, groundLayerMask) != null;

        if (isGrounded) hasDoubleJumped = false;
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(targetHorizontal * speed, rb.linearVelocity.y);

        if (targetHorizontal > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (targetHorizontal < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateAnimation()
    {
        if (!animator) return;

        bool isRunning = Mathf.Abs(targetHorizontal) > 0.1f && isGrounded;
        bool isJumping = !isGrounded && rb.linearVelocity.y > 0;
        bool isFalling = rb.linearVelocity.y < -0.1f && !isGrounded;

        int currentJumpCount = animator.GetInteger("jumpCount");

        if (currentJumpCount > 0) isFalling = false;
        else if (isFalling && !isJumping) animator.SetBool("isFalling", true);

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isFalling);

        if (isGrounded)
        {
            animator.SetInteger("jumpCount", 0);
            animator.SetBool("isFalling", false);
        }
    }

    private System.Collections.IEnumerator ResetJumpCountAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null && !isGrounded && rb.linearVelocity.y < 0)
        {
            animator.SetInteger("jumpCount", 0);
            animator.SetBool("isFalling", true);
        }
    }

    public void ResetPlayer()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        enabled = true;

        if (animator)
        {
            animator.ResetTrigger("Jump");
            animator.SetInteger("jumpCount", 0);
            animator.SetBool("isFalling", false);
        }
    }

    // ===== UI callbacks =====
    private void OnLeftDown()  { leftHeld = true;  isUIActive = true; }
    private void OnLeftUp()    { leftHeld = false; UpdateFromUIHeld(); }
    private void OnRightDown() { rightHeld = true; isUIActive = true; }
    private void OnRightUp()   { rightHeld = false; UpdateFromUIHeld(); }

    private void UpdateFromUIHeld()
    {
        float uiDir = 0f;
        if (leftHeld) uiDir -= 1f;
        if (rightHeld) uiDir += 1f;
        targetHorizontal = uiDir;
        isUIActive = Mathf.Abs(uiDir) > 0.01f;
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (audioManager) audioManager.PlayJumpEffect();
            if (animator)
            {
                animator.SetTrigger("Jump");
                animator.SetInteger("jumpCount", 1);
                animator.SetBool("isFalling", false);
            }
        }
        else if (enableDoubleJump && !hasDoubleJumped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            hasDoubleJumped = true;
            if (audioManager) audioManager.PlayJumpEffect();
            if (animator)
            {
                animator.SetBool("isFalling", false);
                animator.SetTrigger("Jump");
                animator.SetInteger("jumpCount", 2);
                StartCoroutine(ResetJumpCountAfterDelay(0f));
            }
        }
    }
}
