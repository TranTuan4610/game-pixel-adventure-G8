using UnityEngine;

public static class LevelStatsManager
{
    static string KBestScore(int level) => $"L{level}_BestScore";
    static string KBestTime(int level) => $"L{level}_BestTime";

    public static void SubmitRun(int level, int score, float time)
    {
        int bestScore = PlayerPrefs.GetInt(KBestScore(level), 0);
        if (score > bestScore)
            PlayerPrefs.SetInt(KBestScore(level), score);

        float bestTime = PlayerPrefs.GetFloat(KBestTime(level), 0f);
        if (bestTime <= 0f || time < bestTime)
            PlayerPrefs.SetFloat(KBestTime(level), time);

        PlayerPrefs.Save();
    }

    public static int GetBestScore(int level) => PlayerPrefs.GetInt(KBestScore(level), 0);
    public static float GetBestTime(int level) => PlayerPrefs.GetFloat(KBestTime(level), 0f);

    public static string FormatTime(float sec)
    {
        if (sec <= 0f) return "–";
        int m = Mathf.FloorToInt(sec / 60f);
        float s = sec % 60f;
        return $"{m:00}:{s:00.00}";
    }
}
