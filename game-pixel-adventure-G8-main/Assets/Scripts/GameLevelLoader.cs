using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("Level roots theo thứ tự 1→N")]
    public GameObject[] levelRoots;   // Kéo các GameObject Level1, Level2, Level3… vào đây

    public int TotalLevels => levelRoots != null ? levelRoots.Length : 0;

    void Awake()
    {
        int selected = Mathf.Clamp(PlayerPrefs.GetInt("SelectedLevel", 0), 0, TotalLevels - 1);

        // Tắt hết, bật đúng level
        for (int i = 0; i < TotalLevels; i++)
            levelRoots[i].SetActive(i == selected);

        // Lưu tổng level (để UI Win biết kiểm tra)
        PlayerPrefs.SetInt("TotalLevels", TotalLevels);
    }
}
