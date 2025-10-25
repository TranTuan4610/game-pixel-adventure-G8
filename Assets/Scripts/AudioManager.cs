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

    // Property để truy cập background audio source
    public AudioSource MusicBackground => BackGroundAudioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        }
    }
}
