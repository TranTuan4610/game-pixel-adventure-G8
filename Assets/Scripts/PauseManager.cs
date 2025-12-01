using UnityEngine;

[System.Serializable]
public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the pause panel here")]
    [SerializeField] private GameObject backgroundPause;

    private bool isPaused;

    private void Awake()
    {
        if (backgroundPause != null) 
            backgroundPause.SetActive(false);
            
        Time.timeScale = 1f;
        isPaused = false;
    }

    /// <summary>
    /// Call this when pause button is clicked
    /// </summary>
    public void OpenPause()
    {
        if (isPaused) return;
        
        isPaused = true;
        if (backgroundPause != null) 
            backgroundPause.SetActive(true);
            
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Call this when continue button is clicked
    /// </summary>
    public void ClosePause()
    {
        if (!isPaused) return;
        
        isPaused = false;
        if (backgroundPause != null) 
            backgroundPause.SetActive(false);
            
        Time.timeScale = 1f;
    }



    /// <summary>
    /// Reset timescale when object is disabled
    /// </summary>
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
