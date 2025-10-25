using UnityEngine;
using UnityEngine.UI;

public class MusicToggle : MonoBehaviour
{
    public Sprite musicOnSprite;   // icon bật
    public Sprite musicOffSprite;  // icon tắt
    public Image iconImage;        // Image hiển thị icon
    public AudioManager audioManager;

    void Start()
    {
        if (iconImage == null) iconImage = GetComponent<Image>();
        if (audioManager == null) audioManager = FindObjectOfType<AudioManager>();
        SyncIcon();
    }

    // Gọi hàm này từ Button OnClick
    public void OnClickToggle()
    {
        if (audioManager == null) return;
        audioManager.ToggleMusic();
        SyncIcon();
    }

    void SyncIcon()
    {
        bool muted = audioManager != null && audioManager.IsMusicMuted();
        if (iconImage != null)
            iconImage.sprite = muted ? musicOffSprite : musicOnSprite;
    }
}
