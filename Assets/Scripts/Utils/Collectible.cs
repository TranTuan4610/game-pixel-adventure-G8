using UnityEngine;
using PixelAdventure.Managers;
using PixelAdventure.Level;

namespace PixelAdventure.Utils
{
    public class Collectible : MonoBehaviour
    {
        [Header("Collectible Settings")]
        public int scoreValue = 100;
        public bool destroyOnCollect = true;
        public float bobSpeed = 2f;
        public float bobHeight = 0.5f;

        [Header("Effects")]
        public GameObject collectEffect;
        public AudioClip collectSound;

        private Vector3 startPosition;
        private LevelManager levelManager;

        private void Start()
        {
            startPosition = transform.position;
            levelManager = FindObjectOfType<LevelManager>();
        }

        private void Update()
        {
            // Simple bobbing animation
            if (bobHeight > 0)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                CollectItem();
            }
        }

        private void CollectItem()
        {
            // Add score
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(scoreValue);
            }

            // Notify level manager
            if (levelManager != null)
            {
                levelManager.CollectItem();
            }

            // Play collect sound
            if (collectSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(collectSound);
            }
            else if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("Collect");
            }

            // Spawn effect
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Destroy or hide collectible
            if (destroyOnCollect)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
