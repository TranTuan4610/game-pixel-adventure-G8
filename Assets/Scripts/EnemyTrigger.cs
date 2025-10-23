using UnityEngine;
using PixelAdventure.Managers;

public class EnemyTrigger : MonoBehaviour
{
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}
