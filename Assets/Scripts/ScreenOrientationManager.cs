using UnityEngine;

public class ScreenOrientationManager : MonoBehaviour
{
    void Start()
    {
        // Set the default orientation to landscape left
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Allow auto-rotation between landscape left and landscape right
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        
        // Enable full screen auto-rotation
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    void Awake()
{
    DontDestroyOnLoad(gameObject);
}
        
}
