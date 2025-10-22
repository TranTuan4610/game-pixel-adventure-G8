using UnityEngine;

namespace PixelAdventure.Level
{
    // Minimal stub to keep references compiling while LevelManager is disabled
    public class LevelManager : MonoBehaviour
    {
        public System.Action<float> OnTimerUpdated;
        public System.Action<int, int> OnCollectiblesUpdated;

        public void StartLevel() {}
        public void RestartLevel() {}
        public void CollectItem() {}
        public void CompleteLevel() {}

        public float GetRemainingTime() => 0f;
        public int GetCollectedItems() => 0;
        public int GetTotalCollectibles() => 0;
        public bool IsLevelCompleted() => false;
        public bool IsLevelStarted() => false;
    }
}
