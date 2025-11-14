using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PixelAdventure.Managers
{
    public static class SaveManager
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "save_game.json");
        private static SaveData _data;

        public static SaveData Data
        {
            get
            {
                if (_data == null)
                    Load();
                return _data;
            }
        }

        public static void Load()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    _data = JsonUtility.FromJson<SaveData>(json);
                    if (_data == null) // phòng trường hợp FromJson trả về null
                        _data = new SaveData();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"SaveManager.Load failed: {e.Message}");
                    _data = new SaveData();
                }
            }
            else
            {
                _data = new SaveData();
            }
        }

        public static void Save()
        {
            if (_data == null) _data = new SaveData();
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(SavePath, json);
        }

        // ===== Best per level =====
        public static LevelBest GetLevelBest(int levelIndex)
        {
            var best = Data.levels.FirstOrDefault(l => l.levelIndex == levelIndex);
            if (best == null)
            {
                best = new LevelBest { levelIndex = levelIndex, bestScore = 0, bestTime = 0f };
                Data.levels.Add(best);
                Save();
            }
            return best;
        }

        public static void UpdateBestForLevel(int levelIndex, int score, float time)
        {
            var best = GetLevelBest(levelIndex);

            // Best score: lớn hơn là tốt hơn
            if (score > best.bestScore)
                best.bestScore = score;

            // Best time: nhỏ hơn là tốt hơn (0 nghĩa là chưa có)
            if (best.bestTime <= 0f || time < best.bestTime)
                best.bestTime = time;

            Save();
        }

        // ===== Recent 5 runs PER LEVEL =====
        public static void AddRun(int levelIndex, int score, float time)
        {
            var run = new RunRecord
            {
                levelIndex = levelIndex,
                score = score,
                time = time,
            };

            // Tìm hoặc tạo LevelRecentRuns cho level này
            var levelRuns = Data.levelRecentRuns.FirstOrDefault(lr => lr.levelIndex == levelIndex);
            if (levelRuns == null)
            {
                levelRuns = new LevelRecentRuns { levelIndex = levelIndex };
                Data.levelRecentRuns.Add(levelRuns);
            }

            // Thêm run mới vào
            levelRuns.runs.Add(run);

            // Giữ tối đa 5 ván gần nhất cho level này
            while (levelRuns.runs.Count > 5)
            {
                levelRuns.runs.RemoveAt(0); // xoá ván cũ nhất
            }

            Save();
        }

        // Lấy 5 ván gần nhất của một level cụ thể
        public static List<RunRecord> GetRecentRunsForLevel(int levelIndex)
        {
            var levelRuns = Data.levelRecentRuns.FirstOrDefault(lr => lr.levelIndex == levelIndex);
            if (levelRuns == null)
            {
                return new List<RunRecord>();
            }
            return levelRuns.runs;
        }
    }
}
