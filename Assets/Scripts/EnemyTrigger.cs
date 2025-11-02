using UnityEngine;
using PixelAdventure.Managers;

public class EnemyTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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
