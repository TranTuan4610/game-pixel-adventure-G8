using System;
using System.Collections.Generic;

namespace PixelAdventure.Managers
{
    [Serializable]
    public class LevelBest
    {
        public int levelIndex;
        public int bestScore;
        public float bestTime; // thêm trường thời gian tốt nhất
    }

    [Serializable]
    public class RunRecord
    {
        public int levelIndex;
        public int score;
        public float time;
    }

    [Serializable]
    public class LevelRecentRuns
    {
        public int levelIndex;
        public List<RunRecord> runs = new List<RunRecord>();
    }

    [Serializable]
    public class SaveData
    {
        // Đặt tên đúng như SaveManager đang dùng: "levels" và "recentRuns"
        public List<LevelBest> levels = new();

        // DEPRECATED: Giữ lại để tương thích với save cũ, nhưng không dùng nữa
        public List<RunRecord> recentRuns = new();

        // MỚI: Lưu 5 ván gần nhất cho từng level riêng biệt
        public List<LevelRecentRuns> levelRecentRuns = new();
    }
}
