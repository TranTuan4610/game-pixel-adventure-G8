using UnityEngine;
using TMPro;

namespace PixelAdventure.Managers
{
    public class GameManager : MonoBehaviour
    {
        private int score = 0;
        public int currentLives = 3;

        [SerializeField] private TextMeshProUGUI scoreText;

        // ⚠️ Hàm Start phải viết hoa chữ "S"
        void Start()
        {
            UpdateScore();
        }

        // Update is called once per frame
        void Update()
        {
            // (Tùy bạn — nếu không cần cập nhật mỗi frame thì có thể bỏ trống)
        }

        // Hàm cộng điểm
        public void AddScore(int points)
        {
            score += points;
            UpdateScore();
        }

        // Called by gameplay when the player loses a life
        public void LoseLife()
        {
            if (currentLives > 0)
                currentLives--;
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
    }
}
