using UnityEngine;
using System.Collections;
using PixelAdventure.Managers; // Import GameManager namespace

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 5f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float wallCheckDistance = 0.5f;
    
    private Vector3 startPos;
    private bool movingRight = false;
    private Rigidbody2D rb;
    private BoxCollider2D BoxCollider;
    private float lastFlipTime = 0f;
    private float flipCooldown = 0.5f;
    public Animator anim;           // gán nếu có Animator
    public float deathDelay = 0.2f; // thời gian chờ rồi Destroy
    public int scoreValue = 20;    // cộng điểm nếu bạn có GameManager
    bool isDead = false;
    bool isAnimationFinished = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BoxCollider = GetComponent<BoxCollider2D>();
        
        if (groundCheck == null)
        {
            CreateGroundCheck();
        }
        
        CreateTriggerCollider();
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    
    private void CreateGroundCheck()
    {
        GameObject groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.SetParent(transform);
        
        if (BoxCollider != null)
        {
            float bottomY = -BoxCollider.size.y / 2 - 0.1f;
            groundCheckObj.transform.localPosition = new Vector3(0, bottomY, 0);
        }
        else
        {
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        
        groundCheck = groundCheckObj.transform;
    }
    
    private void CreateTriggerCollider()
    {
        GameObject triggerObj = new GameObject("EnemyTrigger");
        triggerObj.transform.SetParent(transform);
        triggerObj.transform.localPosition = Vector3.zero;
        triggerObj.layer = gameObject.layer;
        triggerObj.tag = "Enemy"; // Set tag so PlayerCollision can detect it
        
        BoxCollider2D triggerCollider = triggerObj.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        
        if (BoxCollider != null)
        {
            triggerCollider.size = BoxCollider.size;
            triggerCollider.offset = BoxCollider.offset;
        }
        else
        {
            triggerCollider.size = new Vector2(0.5f, 0.5f);
        }
        
        triggerObj.AddComponent<EnemyTrigger>();
    }

    void Start()
    {
        startPos = transform.position;
        
        if (groundLayerMask.value == 0)
        {
            SetupDefaultLayerMask();
        }
    }
    
    private void SetupDefaultLayerMask()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            groundLayerMask = 1 << groundLayer;
        }
        else
        {
            groundLayerMask = 1 << LayerMask.NameToLayer("Default");
        }
    }
    
    void FixedUpdate()
    {
        if (rb == null || isDead) return; // Không di chuyển khi đã chết

        if (Time.time - lastFlipTime > flipCooldown)
        {
            bool shouldFlip = false;
            
            bool hasGroundAhead = CheckGroundAhead();
            
            float leftBound = startPos.x - distance;
            float rightBound = startPos.x + distance;
            
            // Flip nếu không có ground phía trước (phát hiện hố)
            if (!hasGroundAhead)
            {
                shouldFlip = true;
            }
            // Hoặc flip nếu đã đi đủ xa theo distance
            else if (movingRight && transform.position.x >= rightBound)
            {
                shouldFlip = true;
            }
            else if (!movingRight && transform.position.x <= leftBound)
            {
                shouldFlip = true;
            }
            
            if (shouldFlip)
            {
                movingRight = !movingRight;
                Flip();
                lastFlipTime = Time.time;
            }
        }
        
        float xVel = (movingRight ? 1f : -1f) * speed;
        rb.linearVelocity = new Vector2(xVel, rb.linearVelocity.y);
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Cộng điểm khi kill enemy
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue); // Cộng 20 điểm (theo scoreValue)
        }

        // Trigger animation chết (nếu có)
        if (anim) anim.SetTrigger("Die");

        // Tắt collider để không vướng player
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Tắt EnemyTrigger riêng biệt nếu có
        var enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        if (enemyTrigger) enemyTrigger.enabled = false;

        // Set velocity = 0 để không di chuyển ngang
        if (rb)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // Bắt đầu fade effect - enemy sẽ rơi do gravity
        StartCoroutine(FadeAndDestroy());
    }

    // Gọi từ Animation Event khi animation Die hoàn thành
    public void OnDeathAnimFinished()
    {
        if (isAnimationFinished) return;
        isAnimationFinished = true;

        // Bật gravity để enemy rơi xuống
        if (rb)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.gravityScale = 3f; // Rơi nhanh hơn
        }

        // Bắt đầu fade out
        StartCoroutine(FadeAndDestroy());
    }

    // Method đơn giản - chỉ animation và destroy
    public void SimpleDie()
    {
        if (isDead) return;
        isDead = true;

        // Cộng điểm
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }

        // Trigger animation
        if (anim) anim.SetTrigger("Die");

        // Tắt collider
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        var enemyTrigger = GetComponentInChildren<EnemyTrigger>();
        if (enemyTrigger) enemyTrigger.enabled = false;

        // Tắt physics hoàn toàn
        if (rb)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        // Destroy sau animation (1 giây)
        Destroy(gameObject, 1f);
    }

    IEnumerator FadeAndDestroy()
    {
        // Chờ animation chạy xong (0.5 giây) - nếu không có animation event
        if (!isAnimationFinished)
        {
            yield return new WaitForSeconds(0.5f);

            // Bật gravity để enemy rơi xuống
            if (rb)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                rb.gravityScale = 3f; // Rơi nhanh hơn
            }
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            // Không có sprite renderer, destroy sau 2 giây
            Destroy(gameObject, 2f);
            yield break;
        }

        Color originalColor = sr.color;
        float fadeTime = 1.5f; // Fade trong khi rơi
        float t = 0;

        // Fade out trong khi rơi
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Destroy sau khi fade xong
        Destroy(gameObject);
    }
    
    bool CheckGroundAhead()
    {
        if (BoxCollider == null) return true;

        // Lấy theo world-space để không lệch khi scale
        var b = BoxCollider.bounds;
        float halfWidth = b.extents.x;
        float bottomY = b.min.y;

        // Raycast từ mép trước enemy đi xuống
        float offsetX = halfWidth + 0.1f;
        float rayLen = 0.6f;            // dài hơn 1 chút để chắc ăn
        int checkPoints = 3;            // quét 3 điểm trước mũi

        for (int i = 0; i < checkPoints; i++)
        {
            float extra = i * 0.1f;
            Vector2 origin;

            if (movingRight)
                origin = new Vector2(b.center.x + offsetX + extra, bottomY + 0.05f);
            else
                origin = new Vector2(b.center.x - offsetX - extra, bottomY + 0.05f);

            var hit = Physics2D.Raycast(origin, Vector2.down, rayLen, groundLayerMask);

            Debug.DrawLine(origin, origin + Vector2.down * rayLen, hit.collider ? Color.green : Color.red, 0.02f);

            // Chỉ coi là ground nếu có hit VÀ (là ground HOẶC không phải EnemyHead)
            if (hit.collider != null)
            {
                // Không phải EnemyHead
                if (!hit.collider.CompareTag("Enemy"))
                {
                    // Là ground thật
                    if (hit.collider.CompareTag("Ground") ||
                        hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        return true; // có đất thật phía trước
                    }
                }
            }
        }

        return false; // không còn đất → cần flip
    }

    bool CheckWallAhead()
    {
        if (BoxCollider == null) return false;

        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, groundLayerMask);

        // Chỉ coi là wall nếu không phải EnemyHead
        return hit.collider != null && !hit.collider.CompareTag("Enemy");
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
