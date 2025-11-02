using UnityEngine;


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
            return;
        }

        if (idx < 0 || idx >= playerPrefabs.Length)
        {
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
