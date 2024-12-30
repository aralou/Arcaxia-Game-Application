using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton instance

    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string Level2UnlockedKey = "Level2Unlocked";
    private const string OverallScoreKey = "OverallScore";

    public int UnlockedLevel { get; private set; } = 1; // Default to Level 1 being unlocked
    public GameObject level2Button; // Reference to Level 2 button in the map scene

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
            Destroy(gameObject);
        }
    }

    public void UpdateLevelButtons()
    {
        if (level2Button != null)
        {
            bool isLevel2Unlocked = PlayerPrefs.GetInt(Level2UnlockedKey, 0) == 1;
            level2Button.SetActive(isLevel2Unlocked);
            Debug.Log($"Level 2 button state: {isLevel2Unlocked}");
        }
    }

    // Method to unlock or lock Level 2 based on overall score
    public void CheckAndUnlockLevel2()
    {
        int overallScore = PlayerPrefs.GetInt(OverallScoreKey, 0); // Retrieve overall score
        string performanceCategory = GetScoreCategory(overallScore);

        if (performanceCategory == "Fair" || performanceCategory == "Excellent")
        {
            UnlockLevel2();
        }
        else
        {
            LockLevel2(); // Explicitly lock Level 2 if performance is poor
        }
    }

    private void UnlockLevel2()
    {
        PlayerPrefs.SetInt(Level2UnlockedKey, 1); // Save Level 2 unlocked state
        PlayerPrefs.SetInt(UnlockedLevelKey, 2); // Update progress to Level 2
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 2 has been unlocked!");
    }

    private void LockLevel2()
    {
        PlayerPrefs.SetInt(Level2UnlockedKey, 0); // Save Level 2 locked state
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 2 remains locked due to poor performance.");
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
