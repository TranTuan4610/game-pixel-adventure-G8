using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace PixelAdventure.Managers
{
    public class GameManager : MonoBehaviour
    {
        private int score = 0;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject gameOverUi;
        [SerializeField] private Button restartButton; 
        [SerializeField] private GameObject gameWinUi;
        private bool isGameOver = false;
        private bool isGameWin = false;
        // ⚠️ Hàm Start phải viết hoa chữ "S"
        void Start()
        {
            UpdateScore();
            gameOverUi.SetActive(false);
            gameWinUi.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            // (Tùy bạn — nếu không cần cập nhật mỗi frame thì có thể bỏ trống)
        }

        // Hàm cộng điểm
        public void AddScore(int points)
        {
            if(!isGameOver && !isGameWin)
            {
                score += points;
                UpdateScore();
            }
        }
    

        // Called when the level is completed
        public void CompleteLevel()
        {
            // You can add transitions/UI here later; for now just log
            Debug.Log("Level Completed");
        }

        // Hàm cập nhật điểm hiển thị lên UI
        private void UpdateScore()
        {
            scoreText.text = score.ToString();
        }

        // Hàm mất mạng - game over trực tiếp vì chỉ có 1 mạng
        public void LoseLife()
        {
            GameOver();
        }

        public void GameOver()
        {
            score = 0;
            Time.timeScale = 0;
            gameOverUi.SetActive(true);
        }
        public void GameWin()
        {
            score = 0;
            Time.timeScale = 0;
            gameWinUi.SetActive(true);
        }
        public void RestartGame()
        {
            isGameOver = false;
            score = 0;
            UpdateScore();
            Time.timeScale = 1;
            SceneManager.LoadScene("game");
        }
        public void GoToMenu()
        {
            SceneManager.LoadScene("menu");
            Time.timeScale = 1;
        }
        public bool GetIsGameOver()
        {
            return isGameOver;
        }
        public bool GetIsGameWin()
        {
            return isGameWin;
        }
    }
}
