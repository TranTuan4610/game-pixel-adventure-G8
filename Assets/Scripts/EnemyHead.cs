using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    [Header("Enemy References")]
    public Enemy enemy;               // auto lấy từ parent nếu để trống
    public BlueBird blueBird;         // thêm tham chiếu đến BlueBird
    
    [Header("Bounce Settings")]
    public float bounceForce = 10f;   // lực bật nảy cho player

    void Reset()
    {
        // Tự động tìm cả Enemy và BlueBird component
        enemy = GetComponentInParent<Enemy>();
        blueBird = GetComponentInParent<BlueBird>();
        gameObject.tag = "Enemy";     // Set tag để enemy không detect là ground
    }

    void Start()
    {
        // Tự động tìm nếu chưa được gán
        if (enemy == null && blueBird == null)
        {
            enemy = GetComponentInParent<Enemy>();
            blueBird = GetComponentInParent<BlueBird>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // Kiểm tra va chạm từ trên xuống
        if (IsPlayerComingFromAbove(other))
        {
            // Gọi Die() cho cả Enemy và BlueBird nếu có
            enemy?.Die();
            blueBird?.Die();

            // Bật nảy player lên
            BouncePlayer(rb);
        }
    }

    private bool IsPlayerComingFromAbove(Collider2D playerCollider)
    {
        // Kiểm tra player đang rơi xuống (cho phép một chút velocity dương để tránh miss)
        bool isFalling = playerCollider.attachedRigidbody.linearVelocity.y <= 0.5f;

        // Kiểm tra player chạm từ phía trên
        float playerBottom = playerCollider.bounds.min.y;
        float enemyHeadTop = GetComponent<Collider2D>().bounds.max.y;
        bool hitFromTop = playerBottom >= enemyHeadTop - 0.3f; // Tăng tolerance để dễ stomp hơn

        return isFalling && hitFromTop;
    }

    private void BouncePlayer(Rigidbody2D playerRigidbody)
    {
        Vector2 velocity = playerRigidbody.linearVelocity;
        velocity.y = bounceForce;
        playerRigidbody.linearVelocity = velocity;
    }
}