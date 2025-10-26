using UnityEngine;

/*
 * PlayerSpawner.cs - Spawns the selected character from CharacterSelectUI
 *
 * SETUP:
 * 1. Attach to an empty GameObject in your game scene
 * 2. Assign playerPrefabs[0-3] with your character prefabs
 * 3. Assign spawnPoint (optional - defaults to this object's position)
 * 4. Attach Follow_player.cs to your Main Camera
 * 5. Ensure your player prefabs have tag "Player"
 *
 * Camera Follow Options (in Follow_player.cs):
 * - 2D Side-scrolling: followX=true, followY=false, followZ=true
 * - 2D Top-down: followX=true, followY=true, followZ=false
 * - 3D: followX=true, followY=true, followZ=true
 * - Custom: Set individual followX, followY, followZ options
 */

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs;  // Index 0=Player1, 1=Player2, 2=Player3, 3=Player4
    public Transform spawnPoint;        // Optional spawn position

    void Start()
    {
        // Get selected character from PlayerPrefs (set by CharacterSelectUI)
        int idx = PlayerPrefs.GetInt("SelectedCharacter", 0);

        // Validate selection
        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogError("PlayerSpawner: No player prefabs assigned!");
            return;
        }

        if (idx < 0 || idx >= playerPrefabs.Length)
        {
            Debug.LogWarning($"PlayerSpawner: Invalid character index {idx}, defaulting to Player 1");
            idx = 0;
        }

        // Spawn at specified position or default to this object
        Vector3 pos = spawnPoint ? spawnPoint.position : transform.position;
        GameObject player = Instantiate(playerPrefabs[idx], pos, Quaternion.identity);

        // Set tag for camera follow scripts
        player.tag = "Player";

        // Find camera follow script and assign player
        Follow_player[] followScripts = FindObjectsOfType<Follow_player>();
        foreach (Follow_player followScript in followScripts)
        {
            followScript.SetPlayer(player.transform);
        }
    }
}
