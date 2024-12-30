public static class ProgressTracker
{
    public static int totalPositive = 0; // Tracks positive (Yes) answers
    public static int totalNegative = 0; // Tracks negative (No) answers

    public static int TotalAnswers => totalPositive + totalNegative;

    public static float PositivePercentage => TotalAnswers > 0 ? (float)totalPositive / TotalAnswers : 0f;
    public static float NegativePercentage => TotalAnswers > 0 ? (float)totalNegative / TotalAnswers : 0f;
}
