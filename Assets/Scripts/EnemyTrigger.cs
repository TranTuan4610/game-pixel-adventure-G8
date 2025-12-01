using UnityEngine;
using PixelAdventure.Managers;

public class EnemyTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    private Rigidbody2D playerRb;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Kiểm tra xem player có đang stomp từ trên xuống không
            if (playerRb == null)
            {
                playerRb = collision.attachedRigidbody;
            }

            if (playerRb != null)
            {
                // Nếu player đang rơi xuống và va chạm từ trên
                bool isFalling = playerRb.linearVelocity.y <= 0.5f;
                float playerBottom = collision.bounds.min.y;
                float enemyTop = GetComponent<Collider2D>().bounds.max.y;
                bool hitFromTop = playerBottom >= enemyTop - 0.3f;

                if (isFalling && hitFromTop)
                {
                    // Đây là stomp, không gây game over
                    // EnemyHead sẽ xử lý việc kill enemy
                    return;
                }
            }

            // Nếu không phải stomp, thì game over
            if (audioManager != null)
            {
                audioManager.PlayGameOverEffect();
            }

            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}
