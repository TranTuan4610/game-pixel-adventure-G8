using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    public Enemy enemy;               // auto lấy từ parent nếu để trống
    public float bounceForce = 10f;   // lực bật nảy cho player

    void Reset()
    {
        enemy = GetComponentInParent<Enemy>();
        gameObject.tag = "Enemy";     // Set tag để enemy không detect là ground
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // Chỉ cho giết khi player đang rơi xuống & chạm từ trên
        bool falling = rb.linearVelocity.y <= 0f;
        bool fromAbove = other.bounds.min.y >= transform.position.y;

        if (falling && fromAbove)
        {
            enemy?.Die();

            // Bật nảy player lên
            Vector2 v = rb.linearVelocity;
            v.y = bounceForce;
            rb.linearVelocity = v;
        }
    }
}
