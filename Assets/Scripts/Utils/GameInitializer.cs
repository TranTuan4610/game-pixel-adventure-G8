using UnityEngine;
using PixelAdventure.Managers;

namespace PixelAdventure.Utils
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Manager Prefabs")]
        public GameObject gameManagerPrefab;
        public GameObject audioManagerPrefab;
        public GameObject playerDataManagerPrefab;

        [Header("Settings")]
        public bool initializeOnAwake = true;

        private void Awake()
        {
            if (initializeOnAwake)
            {
                InitializeGame();
            }
        }

        public void InitializeGame()
        {
            // Initialize GameManager
            if (FindObjectOfType<GameManager>() == null && gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }

            // Initialize AudioManager
            if (AudioManager.Instance == null && audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }

            // Initialize PlayerDataManager
            if (PlayerDataManager.Instance == null && playerDataManagerPrefab != null)
            {
                Instantiate(playerDataManagerPrefab);
            }

            // Set up application settings
            SetupApplicationSettings();
        }

        private void SetupApplicationSettings()
        {
            // Set target frame rate for mobile
            #if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 60;
            #endif

            // Don't destroy this initializer
            DontDestroyOnLoad(gameObject);
        }
    }
}
