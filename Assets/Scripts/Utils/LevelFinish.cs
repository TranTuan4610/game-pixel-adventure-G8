using UnityEngine;
using PixelAdventure.Managers;
using PixelAdventure.Level;

namespace PixelAdventure.Utils
{
    public class LevelFinish : MonoBehaviour
    {
        [Header("Finish Settings")]
        public bool requireAllCollectibles = false;
        public GameObject finishEffect;
        public AudioClip finishSound;

        private LevelManager levelManager;
        private bool hasTriggered = false;

        private void Start()
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !hasTriggered)
            {
                if (CanFinishLevel())
                {
                    FinishLevel();
                }
                else
                {
                    // Show message that all collectibles are required
                    Debug.Log("Collect all items before finishing the level!");
                }
            }
        }

        private bool CanFinishLevel()
        {
            if (!requireAllCollectibles)
                return true;

            if (levelManager != null)
            {
                return levelManager.GetCollectedItems() >= levelManager.GetTotalCollectibles();
            }

            return true;
        }

        private void FinishLevel()
        {
            hasTriggered = true;

            // Play finish sound
            if (finishSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(finishSound);
            }
            else if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound("LevelComplete");
            }

            // Spawn effect
            if (finishEffect != null)
            {
                Instantiate(finishEffect, transform.position, Quaternion.identity);
            }

            // Complete level
            if (levelManager != null)
            {
                levelManager.CompleteLevel();
            }
            else if (FindObjectOfType<GameManager>() != null)
            {
                FindObjectOfType<GameManager>().CompleteLevel();
            }
        }
    }
}
