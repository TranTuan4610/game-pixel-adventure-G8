using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;        // panel menu chính
    public GameObject characterPanel;   // panel chọn nhân vật
    public GameObject ChoseLevelPanel;  // panel chọn level
    void Start()
    {
        // Setup auto-rotation cho landscape
        Screen.autorotateToPortrait = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        // bảo đảm ban đầu panel chọn nhân vật tắt
        if (characterPanel != null)
            characterPanel.SetActive(false);
        if (mainPanel) mainPanel.SetActive(true);
        if (ChoseLevelPanel) ChoseLevelPanel.SetActive(false);
    }
    /// <summary>
    /// Hàm này được gọi khi nhấn nút Start
    /// </summary>
    public void StartGame()
    {
        ShowChooseLevel();
    }

    /// <summary>
    /// Hiển thị panel chọn level và ẩn main panel
    /// </summary>
    public void ShowChooseLevel()
    {
        if (ChoseLevelPanel != null)
            ChoseLevelPanel.SetActive(true);
            
        if (mainPanel != null) 
            mainPanel.SetActive(false);
    }
    // Chỉ bật panel chọn nhân vật, KHÔNG tắt menu chính
    public void ShowCharacterPanel()
    {
        if (characterPanel != null)
            characterPanel.SetActive(true);
    }

    // Nút Back sẽ tắt panel chọn nhân vật đi
    public void HideCharacterPanel()
    {
        if (characterPanel != null)
            characterPanel.SetActive(false);
    }
    public void BackToMain()
    {
        if (ChoseLevelPanel != null) 
            ChoseLevelPanel.SetActive(false);
            
        if (mainPanel != null) 
            mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
