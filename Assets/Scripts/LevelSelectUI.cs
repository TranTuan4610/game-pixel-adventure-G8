using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game"; // tên scene chơi chính
    private const string LevelKey = "SelectedLevel";
    private const string CurrentLevelKey = "CurrentLevel";

    /// <summary>
    /// Gán hàm này cho từng nút level: 0,1,2,...
    /// </summary>
    public void SelectLevel(int levelIndex)
    {
        // Lưu cả SelectedLevel (cho LevelLoader) và CurrentLevel (cho hệ thống level)
        PlayerPrefs.SetInt(LevelKey, levelIndex);
        PlayerPrefs.SetInt(CurrentLevelKey, levelIndex);
        PlayerPrefs.Save();
        
        // Load scene game
        SceneManager.LoadScene(gameSceneName);
    }
    
    /// <summary>
    /// Reset về level 1 khi người chơi chọn chơi lại từ đầu
    /// </summary>
    public void ResetToLevelOne()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, 0);
        PlayerPrefs.SetInt(LevelKey, 0);
        PlayerPrefs.Save();
        
        // Load lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
