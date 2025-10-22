using UnityEngine;

namespace PixelAdventure.Managers
{
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SaveLevelProgress(int level)
        {
            PlayerPrefs.SetInt("LevelProgress", Mathf.Max(level, PlayerPrefs.GetInt("LevelProgress", 1)));
            PlayerPrefs.Save();
        }

        public void SaveLevelScore(int level, int score)
        {
            PlayerPrefs.SetInt($"LevelScore_{level}", score);
            PlayerPrefs.Save();
        }

        public void SaveSelectedCharacter(string characterName)
        {
            PlayerPrefs.SetString("SelectedCharacter", characterName);
            PlayerPrefs.Save();
        }
    }
}


