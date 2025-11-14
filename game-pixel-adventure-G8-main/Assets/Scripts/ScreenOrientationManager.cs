using UnityEngine;

public class ScreenOrientationManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Bật đa chạm cho điều khiển trên mobile
        Input.multiTouchEnabled = true;

        // Cho phép auto-rotate nhưng CHỈ 2 hướng ngang
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        // Bật AutoRotation 1 lần là đủ (đừng set mỗi frame)
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
