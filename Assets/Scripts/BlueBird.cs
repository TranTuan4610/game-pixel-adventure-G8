using UnityEngine;
using PixelAdventure.Managers;

[RequireComponent(typeof(Rigidbody2D))]
public class BlueBird : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float flySpeed = 2f;
    [SerializeField] private float moveDistance = 5f; // Khoảng cách di chuyển từ vị trí bắt đầu
    
    private Vector2 startPosition;
    private bool movingRight = true;
    private Rigidbody2D rb;
    private bool isDead = false;
    private bool isFacingRight = true;
    private float lastAttackTime;
    private Animator animator;

    [Header("Head Stomp")]
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private Transform headCheck;
    [SerializeField] private LayerMask playerLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        
        // Bắt đầu bay sang trái
        movingRight = false;
        rb.linearVelocity = Vector2.left * flySpeed;
        
        // Lật sprite để nhìn sang trái (trái = dương)
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x); // Positive x = facing left
        transform.localScale = scale;
        isFacingRight = false;
    }

    private void Update()
    {
        if (isDead) return;
        
        // Tính toán khoảng cách từ vị trí bắt đầu
        float currentDistance = transform.position.x - startPosition.x;
        
        // Đổi hướng khi đạt giới hạn
        if ((movingRight && currentDistance >= moveDistance) || 
            (!movingRight && currentDistance <= -moveDistance))
        {
            movingRight = !movingRight;
            startPosition = transform.position; // Reset vị trí bắt đầu mới
            
            // Cập nhật vận tốc theo hướng mới
            rb.linearVelocity = movingRight ? 
                Vector2.right * flySpeed : 
                Vector2.left * flySpeed;
            
            // Lật sprite (trái = dương, phải = âm)
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (movingRight ? -1 : 1);
            transform.localScale = scale;
        }
        
        // Cập nhật animation nếu có
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    // Gọi khi bị stomp (từ EnemyHead)
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Cộng 15 điểm khi kill BlueBird
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(25); // Cộng 15 điểm cho BlueBird
        }

        // Play animation / vfx
        if (animator != null) animator.SetTrigger("Die");

        // Disable tất cả collider để không gây tương tác thêm
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Bật lại gravity để rơi xuống
        rb.gravityScale = 2f;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Chỉ giữ rotation, cho phép rơi

        // Hủy object sau khi rơi xuống (thời gian dài hơn để rơi hết)
        Destroy(gameObject, 3f);
    }

    // Vẽ Gizmo để dễ dàng điều chỉnh trong Editor
    private void OnDrawGizmosSelected()
    {
        // Vẽ đường đi dự kiến
        Vector3 startPoint = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Vector3 leftPoint = startPoint - Vector3.right * moveDistance;
        Vector3 rightPoint = startPoint + Vector3.right * moveDistance;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftPoint, rightPoint);
        
        // Vẽ điểm bắt đầu và kết thúc
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPoint, 0.2f);
        Gizmos.DrawWireSphere(leftPoint, 0.15f);
        Gizmos.DrawWireSphere(rightPoint, 0.15f);
        
        // Vẽ mũi tên hướng di chuyển ban đầu
        Gizmos.color = Color.red;
        Vector3 arrowStart = startPoint + Vector3.right * 0.5f;
        Gizmos.DrawLine(arrowStart, arrowStart + Vector3.right * 0.5f);
        Gizmos.DrawLine(arrowStart + Vector3.right * 0.5f, arrowStart + Vector3.right * 0.3f + Vector3.up * 0.2f);
        Gizmos.DrawLine(arrowStart + Vector3.right * 0.5f, arrowStart + Vector3.right * 0.3f + Vector3.down * 0.2f);
    }
}
