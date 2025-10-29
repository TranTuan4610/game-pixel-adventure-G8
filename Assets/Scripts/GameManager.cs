using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PixelAdventure.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("=== UI References ===")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject gameOverUi;
        [SerializeField] private GameObject gameWinUi;

        [Header("Win Panel Texts")]
        [SerializeField] private TMP_Text winScoreText;
        [SerializeField] private TMP_Text winYourTimeText;
        [SerializeField] private TMP_Text winHighScoreText;
        [SerializeField] private TMP_Text winBestTimeText;

        [Header("Win Panel Buttons")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button mainMenuButton;

        [Header("=== Game/Level Settings ===")]
        [SerializeField] private int totalLevels = 3;
        [SerializeField] private string menuSceneName = "Menu";

        // ----- State -----
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsGameWin { get; private set; }
        public bool IsTimerRunning { get; private set; }

        private float elapsedTime = 0f;
        private int CurrentLevelIndex => PlayerPrefs.GetInt("SelectedLevel", 0);

        // ==========================
        private void Awake()
        {
            if (gameOverUi) gameOverUi.SetActive(false);
            if (gameWinUi) gameWinUi.SetActive(false);

            if (nextLevelButton) nextLevelButton.onClick.AddListener(NextLevel);
            if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMenu);
        }

        private void Start()
        {
            Time.timeScale = 1f;

            // Orientation setup
            Screen.autorotateToPortrait = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;

            // Reset states
            Score = 0;
            IsGameOver = false;
            IsGameWin = false;
            IsTimerRunning = true;
            elapsedTime = 0f;

            UpdateScoreUI();
            UpdateTimerUI(0f);

            ActivateOnlyCurrentLevelInScene();

            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
        }

        private void Update()
        {
            if (!IsTimerRunning || IsGameOver || IsGameWin) return;
            elapsedTime += Time.deltaTime;
            UpdateTimerUI(elapsedTime);
        }

        // ==========================
        public void AddScore(int points)
        {
            if (IsGameOver || IsGameWin) return;
            Score += points;
            UpdateScoreUI();
        }

        public void GameOver()
        {
            OnGameOver();
        }

        public void GameWin()
        {
            OnWin();
        }

        public void OnGameOver()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            IsTimerRunning = false;
            Time.timeScale = 0f;
            if (gameOverUi) gameOverUi.SetActive(true);
        }

        public void OnWin()
        {
            if (IsGameWin) return;
            IsGameWin = true;
            IsTimerRunning = false;
            Time.timeScale = 0f;

            SaveBestForCurrentLevel(Score, elapsedTime);

            if (winScoreText) winScoreText.text = Score.ToString("000");
            if (winYourTimeText) winYourTimeText.text = FormatTime(elapsedTime);
            if (winHighScoreText) winHighScoreText.text = GetBestScore(CurrentLevelIndex).ToString("000");
            if (winBestTimeText) winBestTimeText.text = FormatTime(GetBestTime(CurrentLevelIndex));

            bool hasNext = CurrentLevelIndex + 1 < totalLevels;
            if (nextLevelButton) nextLevelButton.gameObject.SetActive(hasNext);

            if (gameWinUi) gameWinUi.SetActive(true);
        }

        // ==========================
        private void UpdateScoreUI()
        {
            if (scoreText) scoreText.text = Score.ToString("000");
        }

        private void UpdateTimerUI(float t)
        {
            if (timerText) timerText.text = FormatTime(t);
        }

        public static string FormatTime(float sec)
        {
            int m = Mathf.FloorToInt(sec / 60f);
            int s = Mathf.FloorToInt(sec % 60f);
            return $"{m:00}:{s:00}";
        }

        // ==========================
        private string KeyBestScore(int level) => $"L{level}_BestScore";
        private string KeyBestTime(int level) => $"L{level}_BestTime";

        private void SaveBestForCurrentLevel(int score, float time)
        {
            int lv = CurrentLevelIndex;

            int bestScore = PlayerPrefs.GetInt(KeyBestScore(lv), 0);
            if (score > bestScore)
                PlayerPrefs.SetInt(KeyBestScore(lv), score);

            float bestTime = PlayerPrefs.GetFloat(KeyBestTime(lv), 0f);
            if (bestTime <= 0f || time < bestTime)
                PlayerPrefs.SetFloat(KeyBestTime(lv), time);

            PlayerPrefs.Save();
        }

        private int GetBestScore(int level) => PlayerPrefs.GetInt(KeyBestScore(level), 0);
        private float GetBestTime(int level) => PlayerPrefs.GetFloat(KeyBestTime(level), 0f);

        // ==========================
        private void ActivateOnlyCurrentLevelInScene()
        {
            for (int i = 0; i < totalLevels; i++)
            {
                var go = GameObject.Find($"level{i}");
                if (go) go.SetActive(i == CurrentLevelIndex);
            }
        }

        // ==========================
        #region === EXTRA FUNCTIONS ===

        public void RestartGame()
        {
            Time.timeScale = 1f;
            IsGameOver = false;
            IsGameWin = false;
            Score = 0;
            elapsedTime = 0f;
            IsTimerRunning = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextLevel()
        {
            Time.timeScale = 1f;
            int next = CurrentLevelIndex + 1;
            if (next >= totalLevels)
                return;

            PlayerPrefs.SetInt("SelectedLevel", next);
            PlayerPrefs.Save();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        #endregion

        // Dành cho PlayerController
        public bool GetIsGameOver() => IsGameOver;
        public bool GetIsGameWin() => IsGameWin;
    }
}
