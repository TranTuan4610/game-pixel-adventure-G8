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

        [Header("Game Over Panel Texts")]
        [SerializeField] private TMP_Text[] gameOverRecentRunTexts = new TMP_Text[5];
        [SerializeField] private TMP_Text[] gameOverTimeTexts = new TMP_Text[5];

        [Header("Recent 5 Runs (optional)")]
        [SerializeField] private TMP_Text[] recentRunTexts; 
        // Kéo tối đa 5 TMP_Text tương ứng 5 dòng "ván gần nhất" 
        [Header("Kéo 5 TMP_Text cột Time vào đây")]
        [SerializeField] private TMP_Text[] timeTexts = new TMP_Text[5];

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
        private static bool isLoadingNextLevel = false;


                private void Awake()

                {
                    isLoadingNextLevel = false;

                    if (gameOverUi) gameOverUi.SetActive(false);

                    if (gameWinUi) gameWinUi.SetActive(false);

        

                    // Load save JSON lúc game khởi động

                    SaveManager.Load();

                }

        private void Start()
        {
            Time.timeScale = 1f;

            // Orientation setup (tuỳ bạn)
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
            Input.multiTouchEnabled = true;
        }

        private void Update()
        {
            if (!IsTimerRunning || IsGameOver || IsGameWin) return;
            elapsedTime += Time.deltaTime;
            UpdateTimerUI(elapsedTime);
        }

        // ================== GAME FLOW ==================
        public void AddScore(int points)
        {
            if (IsGameOver || IsGameWin) return;
            Score += points;
            UpdateScoreUI();
        }

        public void GameOver() => OnGameOver();
        public void GameWin() => OnWin();

        public void OnGameOver()
        {
            if (IsGameOver) return;
            
            // Set game state
            IsGameOver = true;
            IsTimerRunning = false;
            
            // Stop all audio
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in allAudioSources)
            {
                audioSource.Stop();
            }
            
            // Stop all coroutines on this object
            StopAllCoroutines();
            
            // Stop all animations
            Animator[] animators = FindObjectsOfType<Animator>();
            foreach (Animator anim in animators)
            {
                anim.enabled = false;
            }
            
            // Stop all movement and physics
            Rigidbody2D[] rigidbodies = FindObjectsOfType<Rigidbody2D>();
            foreach (Rigidbody2D rb in rigidbodies)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }
            
            // Finally, stop time
            Time.timeScale = 0f;
            
            // Update Recent Runs UI for Game Over panel
            if (gameOverRecentRunTexts != null && gameOverRecentRunTexts.Length > 0)
            {
                // Clear previous texts
                foreach (var textMesh in gameOverRecentRunTexts)
                {
                    if (textMesh) textMesh.text = "";
                }
                foreach (var timeText in gameOverTimeTexts)
                {
                    if (timeText) timeText.text = "";
                }

                // Lấy 5 ván gần nhất của level hiện tại
                var recentRuns = SaveManager.GetRecentRunsForLevel(CurrentLevelIndex);

                // Display recent runs, up to 5 (từ mới nhất đến cũ nhất)
                for (int i = 0; i < recentRuns.Count && i < gameOverRecentRunTexts.Length; i++)
                {
                    RunRecord run = recentRuns[recentRuns.Count - 1 - i]; // Lấy từ mới nhất
                    if (i < gameOverRecentRunTexts.Length && gameOverRecentRunTexts[i] != null)
                    {
                        gameOverRecentRunTexts[i].text = $"{run.score}";
                    }
                    if (i < gameOverTimeTexts.Length && gameOverTimeTexts[i] != null)
                    {
                        gameOverTimeTexts[i].text = $"{run.time:F2}s";
                    }
                }
            }

            // Tắt Game Win UI nếu đang bật
            if (gameWinUi) gameWinUi.SetActive(false);
            
            if (gameOverUi)
            {
                // Tìm và kích hoạt tất cả Game Over UI trong scene
                GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.Contains("GameOver") || obj.name.Contains("Game Over"))
                    {
                        obj.SetActive(true);
                        
                        // Bật tất cả parent
                        Transform parent = obj.transform.parent;
                        while (parent != null)
                        {
                            parent.gameObject.SetActive(true);
                            parent = parent.parent;
                        }
                    }
                }
                
                // Bật tất cả parent của gameOverUi
                Transform parentTransform = gameOverUi.transform.parent;
                while (parentTransform != null)
                {
                    parentTransform.gameObject.SetActive(true);
                    parentTransform = parentTransform.parent;
                }
                
                gameOverUi.SetActive(true);
                
                // Đảm bảo Game Over UI ở trên cùng
                Canvas canvas = gameOverUi.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.sortingOrder = 100;
                }
                
                // Kiểm tra và bật CanvasGroup nếu có
                CanvasGroup canvasGroup = gameOverUi.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                
                // Đưa lên cuối trong hierarchy để render trên cùng
                gameOverUi.transform.SetAsLastSibling();
            }
        }

        public void OnWin()
        {
            if (IsGameWin || IsGameOver) return;
            IsGameWin = true;
            IsTimerRunning = false;
            Time.timeScale = 0f;

            // Submit run and save data
            LevelStatsManager.SubmitRun(CurrentLevelIndex, Score, elapsedTime);

            // Update Win UI
            if (gameWinUi) gameWinUi.SetActive(true);
            if (winScoreText) winScoreText.text = Score.ToString();
            if (winYourTimeText) winYourTimeText.text = elapsedTime.ToString("F2") + "s";
            if (winHighScoreText) winHighScoreText.text = LevelStatsManager.GetBestScore(CurrentLevelIndex).ToString();
            if (winBestTimeText) winBestTimeText.text = LevelStatsManager.GetBestTime(CurrentLevelIndex).ToString("F2") + "s";

            // Update Recent Runs UI
            UpdateRecentRunsUI();

            // Check for next level
            if (nextLevelButton)
            {
                nextLevelButton.interactable = (CurrentLevelIndex + 1 < totalLevels);
            }
        }

        private void UpdateRecentRunsUI()
        {
            if (recentRunTexts == null || recentRunTexts.Length == 0) return;

            // Clear previous texts
            foreach (var textMesh in recentRunTexts)
            {
                if (textMesh) textMesh.text = "";
            }
            foreach (var timeText in timeTexts)
            {
                if (timeText) timeText.text = "";
            }

            // Lấy 5 ván gần nhất của level hiện tại
            var recentRuns = SaveManager.GetRecentRunsForLevel(CurrentLevelIndex);

            // Display recent runs, up to 5 (từ mới nhất đến cũ nhất)
            for (int i = 0; i < recentRuns.Count && i < recentRunTexts.Length; i++)
            {
                RunRecord run = recentRuns[recentRuns.Count - 1 - i]; // Lấy từ mới nhất
                if (recentRunTexts[i])
                {
                    recentRunTexts[i].text = $"{run.score}";
                }
                if (i < timeTexts.Length && timeTexts[i] != null)
                {
                    timeTexts[i].text = $"{run.time:F2}s";
                }
            }
        }

        // ================== UI & TIMER ==================
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

        // ================== LEVEL / SCENE ==================
        private void ActivateOnlyCurrentLevelInScene()
        {
            for (int i = 0; i < totalLevels; i++)
            {
                var go = GameObject.Find($"level{i}");
                if (go) go.SetActive(i == CurrentLevelIndex);
            }
        }

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
                    Debug.Log($"[NextLevel] Called. CurrentLevelIndex: {CurrentLevelIndex}");
                    Time.timeScale = 1f;  // Ensure game is not paused
                    int next = CurrentLevelIndex + 1;  // Get next level index
                    Debug.Log($"[NextLevel] 'next' level is: {next}");
                    if (next >= totalLevels)  // Check if there are more levels
                        return;
        
                    PlayerPrefs.SetInt("SelectedLevel", next);  // Save next level
                    PlayerPrefs.Save();  // Save the preference
        
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
                }        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        // Cho PlayerController
        public bool GetIsGameOver() => IsGameOver;
        public bool GetIsGameWin() => IsGameWin;
    }
}
