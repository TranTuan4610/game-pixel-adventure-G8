using UnityEngine;
using PixelAdventure.Managers;

public class PlayerCollision : MonoBehaviour
{
    public GameManager gameManager;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
   private void OnTriggerEnter2D(Collider2D collision)
   {
    if (collision.CompareTag("Coin"))
    {
        Destroy(collision.gameObject);
        gameManager.AddScore(10);
    }
    else if (collision.CompareTag("Trap"))
    {
        gameManager.GameOver(); 
    }
   }
}
