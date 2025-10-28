using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject backgroundPause; // Panel Pause (BackgroundPause)

    bool isPaused;

    void Awake()
    {
        if (backgroundPause) backgroundPause.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Gán vào nút PauseUi
    public void OpenPause()
    {
        if (isPaused) return;
        isPaused = true;
        if (backgroundPause) backgroundPause.SetActive(true);
        Time.timeScale = 0f;                // dừng gameplay
    }

    // Gán vào nút Continue trong BackgroundPause
    public void ClosePause()
    {
        if (!isPaused) return;
        isPaused = false;
        if (backgroundPause) backgroundPause.SetActive(false);
        Time.timeScale = 1f;                // chạy lại
    }



    // Phòng hờ: nếu object bị disable, trả timeScale về 1
    void OnDisable()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}
