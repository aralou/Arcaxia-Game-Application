using UnityEngine;
using UnityEngine.UI;

public class LevelManager4 : MonoBehaviour
{
    public static LevelManager4 Instance; // Singleton instance specific to LevelManager2

    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string Level5UnlockedKey = "Level4Unlocked";
    private const string OverallScoreKey = "OverallScore";

    public int UnlockedLevel { get; private set; } = 1; // Default to Level 1 being unlocked
    public GameObject level5Button; // Reference to Level 4 button in the map scene

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved unlocked level state
            UnlockedLevel = PlayerPrefs.GetInt(UnlockedLevelKey, 1);

            UpdateLevelButtons();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance of this manager exists
        }
    }

    public void UpdateLevelButtons()
    {
        if (level5Button != null)
        {
            bool isLevel5Unlocked = PlayerPrefs.GetInt(Level5UnlockedKey, 0) == 1;
            level5Button.SetActive(isLevel5Unlocked);
            Debug.Log($"Level 5 button state: {isLevel5Unlocked}");
        }
    }

    // Method to unlock or lock Level 3 based on overall score
    public void CheckAndUnlockLevel5()
    {
        int overallScore = PlayerPrefs.GetInt(OverallScoreKey, 0); // Retrieve overall score
        string performanceCategory = GetScoreCategory(overallScore);

        if (performanceCategory == "Fair" || performanceCategory == "Excellent")
        {
            UnlockLevel5();
        }
        else
        {
            LockLevel5(); // Explicitly lock Level 3 if performance is poor
        }
    }

    private void UnlockLevel5()
    {
        PlayerPrefs.SetInt(Level5UnlockedKey, 1); // Save Level 3 unlocked state
        PlayerPrefs.SetInt(UnlockedLevelKey, 5); // Update progress to Level 3
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 5 has been unlocked!");
    }

    private void LockLevel5()
    {
        PlayerPrefs.SetInt(Level5UnlockedKey, 0); // Save Level 3 locked state
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 5 remains locked due to poor performance.");
    }

    // Determine score category (Poor, Fair, Excellent)
    private string GetScoreCategory(int finalScore)
    {
        if (finalScore <= 100)
        {
            return "Poor";
        }
        else if (finalScore <= 220)
        {
            return "Fair";
        }
        else
        {
            return "Excellent";
        }
    }
}
