using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;        // panel menu chính
    public GameObject characterPanel;   // panel chọn nhân vật

    void Start()
    {
        // bảo đảm ban đầu panel chọn nhân vật tắt
        if (characterPanel != null)
            characterPanel.SetActive(false);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
