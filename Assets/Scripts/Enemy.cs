using UnityEngine;

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
    private BoxCollider2D boxCollider;
    private float lastFlipTime = 0f;
    private float flipCooldown = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        
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
        
        if (boxCollider != null)
        {
            float bottomY = boxCollider.bounds.min.y - transform.position.y - 0.1f;
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
        
        BoxCollider2D triggerCollider = triggerObj.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        
        if (boxCollider != null)
        {
            triggerCollider.size = boxCollider.size;
            triggerCollider.offset = boxCollider.offset;
        }
        else
        {
            triggerCollider.size = new Vector2(1f, 1f);
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
        if (rb == null) return;
        
        if (Time.time - lastFlipTime > flipCooldown)
        {
            bool shouldFlip = false;
            
            bool hasGroundAhead = CheckGroundAhead();
            bool hasWallAhead = CheckWallAhead();
            
            float leftBound = startPos.x - distance;
            float rightBound = startPos.x + distance;
            
            if (!hasGroundAhead || hasWallAhead)
            {
                shouldFlip = true;
            }
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
    
    bool CheckGroundAhead()
    {
        if (boxCollider == null) return true;
        
        float offsetX = boxCollider.bounds.extents.x + 0.2f;
        Vector2 frontCheckPos;
        
        if (groundCheck != null)
        {
            frontCheckPos = (Vector2)groundCheck.position + (movingRight ? Vector2.right : Vector2.left) * offsetX;
        }
        else
        {
            float bottomY = boxCollider.bounds.min.y;
            frontCheckPos = new Vector2(
                transform.position.x + (movingRight ? offsetX : -offsetX),
                bottomY
            );
        }
        
        bool hasGround = Physics2D.OverlapCircle(frontCheckPos, groundCheckRadius, groundLayerMask) != null;
        
        return hasGround;
    }
    
    bool CheckWallAhead()
    {
        if (boxCollider == null) return false;
        
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        Vector2 origin = (Vector2)transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, wallCheckDistance, groundLayerMask);
        
        return hit.collider != null;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
