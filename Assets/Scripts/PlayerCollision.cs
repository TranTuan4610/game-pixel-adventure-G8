using UnityEngine;
using PixelAdventure.Managers;

public class PlayerCollision : MonoBehaviour
{
    public GameManager gameManager;
    public AudioManager audioManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
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
