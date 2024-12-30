using UnityEngine;

public static class ScoreManager
{
    public static int totalScore = 0; // Track the total score across all NPCs

    public static void AddScore(int points)
    {
        totalScore += points;
        Debug.Log($"ScoreManager: Added {points} points. Total score is now {totalScore}.");
    }

    public static void SubtractScore(int points)
    {
        totalScore -= points;
        Debug.Log($"ScoreManager: Subtracted {points} points. Total score is now {totalScore}.");
    }

    public static int GetTotalScore()
    {
        Debug.Log($"ScoreManager: Getting total score: {totalScore}");
        return totalScore;
    }

    public static void ResetScore()
    {
        totalScore = 0;
        Debug.Log("ScoreManager: Resetting score to 0.");
    }
}
