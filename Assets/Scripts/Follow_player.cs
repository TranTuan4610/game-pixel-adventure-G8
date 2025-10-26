using UnityEngine;

public class Follow_player : MonoBehaviour
{
    public Transform player;              // Kéo player vào đây (optional)
    public string playerTag = "Player";   // Tag để tự tìm player
    public Vector3 offset = new Vector3(0, 1, -5); // Offset từ player
    public float smoothSpeed = 0.125f;    // Tốc độ mượt mà

    // Options để follow theo từng trục
    public bool followX = true;           // Follow theo trục X
    public bool followY = false;          // Follow theo trục Y
    public bool followZ = true;           // Follow theo trục Z

    // Presets cho các loại game (chỉ cần tick 1 trong 3)
    public bool preset2DSideScrolling = false;  // X=true, Y=false, Z=true
    public bool preset2DTopDown = false;        // X=true, Y=true, Z=false
    public bool preset3D = false;               // X=true, Y=true, Z=true

    void Start()
    {
        // Áp dụng presets nếu được tick
        if (preset2DSideScrolling)
        {
            followX = true; followY = false; followZ = true;
        }
        else if (preset2DTopDown)
        {
            followX = true; followY = true; followZ = false;
        }
        else if (preset3D)
        {
            followX = true; followY = true; followZ = true;
        }

        // Nếu chưa gán player, thử tìm bằng tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            // Thử tìm player mỗi frame nếu chưa có
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }

        // Tính vị trí muốn đến
        Vector3 desiredPosition = player.position + offset;

        // Giữ nguyên các trục không muốn follow
        if (!followX) desiredPosition.x = transform.position.x;
        if (!followY) desiredPosition.y = transform.position.y;
        if (!followZ) desiredPosition.z = transform.position.z;

        // Smooth transition
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Áp dụng vị trí
        transform.position = smoothedPosition;
    }

    // Method để gán player từ script khác
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}  
