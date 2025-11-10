using UnityEngine;
using PixelAdventure.Managers;

public class PlayerCollision : MonoBehaviour
{
    public GameManager gameManager;
    public AudioManager audioManager;

    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
        playerRb = GetComponentInParent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            playerCollider = GetComponentInParent<Collider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            // Cộng điểm và âm thanh trước
            gameManager.AddScore(10);
            audioManager.PlayCoinEffect();

            // Tắt collider để tránh va chạm lại
            collision.enabled = false;

            // Trigger animation từ Coin script
            Coin coinScript = collision.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.Collect();
            }
            else
            {
                // Fallback - destroy ngay nếu không có Coin script
                Destroy(collision.gameObject);
            }
        }
        else if (collision.CompareTag("Trap") || collision.CompareTag("Enemy"))
        {
            if (collision.CompareTag("Enemy"))
            {
                if (playerRb != null && playerCollider != null)
                {
                    // Player is falling or jumping slightly up (tăng tolerance để dễ stomp hơn)
                    bool isFalling = playerRb.linearVelocity.y <= 0.5f;

                    // Player's bottom is above the enemy's top
                    float playerBottom = playerCollider.bounds.min.y;
                    float enemyTop = collision.bounds.max.y;
                    bool hitFromTop = playerBottom >= enemyTop - 0.3f; // Tăng tolerance để dễ stomp hơn

                    if (isFalling && hitFromTop)
                    {
                        // This is a stomp, which is handled by EnemyHead.cs.
                        // So, we do nothing here and let EnemyHead handle the kill and bounce.
                        return;
                    }
                }
            }

            gameManager.GameOver();
            audioManager.PlayGameOverEffect();
        }
        else if (collision.CompareTag("End"))
        {
            gameManager.GameWin();
            audioManager.PlayWinEffect();
        }
    }
}
