using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource BackGroundAudioSource;
    [SerializeField] private AudioSource EffectAudioSource;
    [SerializeField] private AudioClip BackGroundClip;
    [SerializeField] private AudioClip JumpClip;
    [SerializeField] private AudioClip CoinClip;
    [SerializeField] private AudioClip WinClip;
    [SerializeField] private AudioClip GameOverClip;

    private const string MusicMutedKey = "MusicMuted";

    // Property để truy cập background audio source
    public AudioSource MusicBackground => BackGroundAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadMusicState();
        PlayBackGroundMusic();
    }

    public void PlayBackGroundMusic()
    {
        BackGroundAudioSource.clip = BackGroundClip;
        BackGroundAudioSource.Play();
    }
    public void PlayCoinEffect()
    {
        EffectAudioSource.clip = CoinClip;
        EffectAudioSource.Play();
    }
    public void PlayJumpEffect()
    {
        EffectAudioSource.clip = JumpClip;
        EffectAudioSource.Play();
    }
    public void PlayWinEffect()
    {
        EffectAudioSource.clip = WinClip;
        EffectAudioSource.Play();
    }
    public void PlayGameOverEffect()
    {
        EffectAudioSource.clip = GameOverClip;
        EffectAudioSource.Play();
    }
    public bool IsMusicMuted()
    {
        return MusicBackground != null && MusicBackground.mute;
    }

    public void ToggleMusic()
    {
        if (BackGroundAudioSource != null)
        {
            BackGroundAudioSource.mute = !BackGroundAudioSource.mute;
            SaveMusicState();
        }
    }

    private void LoadMusicState()
    {
        if (BackGroundAudioSource != null)
        {
            bool isMuted = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
            BackGroundAudioSource.mute = isMuted;
        }
    }

    private void SaveMusicState()
    {
        if (BackGroundAudioSource != null)
        {
            int mutedValue = BackGroundAudioSource.mute ? 1 : 0;
            PlayerPrefs.SetInt(MusicMutedKey, mutedValue);
            PlayerPrefs.Save();
        }
    }
}
