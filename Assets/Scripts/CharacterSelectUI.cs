using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectUI_Image : MonoBehaviour
{
    [Header("4 nút SELECT (trái → phải)")]
    public Button[] selectButtons;         // Kéo 4 Button

    [Header("Image hiển thị trên nút (optional)")]
    public Image[] buttonImages;           // Nếu để trống, script tự lấy

    [Header("Sprites cho nút")]
    public Sprite selectSprite;            // Ảnh SELECT
    public Sprite selectedSprite;          // Ảnh SELECTED

    [Header("Ảnh đại diện (Avatar từng nhân vật)")]
    public Image[] characterImages;        // Ảnh đại diện player1,2,3,4
    public Sprite[] characterSprites;      // Sprite idle của mỗi nhân vật

    [Header("Start")]
    public Button startButton;
    public string gameSceneName = "Game";

    private const string Key = "SelectedCharacter";
    private int selected = -1;

    void Awake()
    {
        // Tự lấy Image nếu bạn chưa kéo
        if (buttonImages == null || buttonImages.Length != selectButtons.Length)
        {
            buttonImages = new Image[selectButtons.Length];
            for (int i = 0; i < selectButtons.Length; i++)
                if (selectButtons[i])
                    buttonImages[i] = selectButtons[i].GetComponent<Image>();
        }

        // Loại bỏ hiệu ứng màu của Button
        foreach (var btn in selectButtons)
            if (btn) btn.transition = Selectable.Transition.None;

        // Gán sự kiện click
        for (int i = 0; i < selectButtons.Length; i++)
        {
            int idx = i;
            selectButtons[i].onClick.AddListener(() => OnPick(idx));
        }
    }

    void Start()
    {
        // Gán ảnh đại diện cho từng ô
        for (int i = 0; i < characterImages.Length; i++)
        {
            if (i < characterSprites.Length && characterImages[i])
                characterImages[i].sprite = characterSprites[i];
        }

        // Lấy lựa chọn cũ
        selected = PlayerPrefs.GetInt(Key, -1);
        RefreshUI();
    }

    public void OnPick(int idx)
    {
        selected = idx;
        PlayerPrefs.SetInt(Key, selected);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void OnStartGame()
    {
        if (selected < 0) return;
        SceneManager.LoadScene(gameSceneName);
    }

    void RefreshUI()
    {
        // Đổi sprite của nút
        for (int i = 0; i < buttonImages.Length; i++)
        {
            var img = buttonImages[i];
            if (!img) continue;

            bool isSel = (i == selected);
            img.sprite = isSel ? selectedSprite : selectSprite;

        }

        // Làm mờ/hiện rõ ảnh đại diện
        for (int i = 0; i < characterImages.Length; i++)
        {
            if (!characterImages[i]) continue;

            if (i == selected)
                characterImages[i].color = new Color(1f, 1f, 1f, 1f);     // sáng rõ
            else
                characterImages[i].color = new Color(1f, 1f, 1f, 0.4f);   // mờ đi
        }

        if (startButton) startButton.interactable = (selected >= 0);
    }
}
