using UnityEngine;

public static class LevelStatsManager
{
    // Gửi 1 lượt chơi: cập nhật best và thêm vào recent runs
    public static void SubmitRun(int level, int score, float time)
    {
        // dùng SaveManager đã chuẩn hóa ở trên
        PixelAdventure.Managers.SaveManager.UpdateBestForLevel(level, score, time);
        PixelAdventure.Managers.SaveManager.AddRun(level, score, time);
    }

    public static int GetBestScore(int level)
    {
        var best = PixelAdventure.Managers.SaveManager.GetLevelBest(level);
        return best?.bestScore ?? 0;
    }

    public static float GetBestTime(int level)
    {
        var best = PixelAdventure.Managers.SaveManager.GetLevelBest(level);
        return best?.bestTime ?? 0f;
    }
}
