using UnityEngine;
using UnityEngine.UI;

public class LevelManager2 : MonoBehaviour
{
    public static LevelManager2 Instance; // Singleton instance specific to LevelManager2

    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string Level3UnlockedKey = "Level3Unlocked";
    private const string OverallScoreKey = "OverallScore";

    public int UnlockedLevel { get; private set; } = 1; // Default to Level 1 being unlocked
    public GameObject level3Button; // Reference to Level 3 button in the map scene

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved unlocked level state
            UnlockedLevel = PlayerPrefs.GetInt(UnlockedLevelKey, 2);

            UpdateLevelButtons();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance of this manager exists
        }
    }

    public void UpdateLevelButtons()
    {
        if (level3Button != null)
        {
            bool isLevel3Unlocked = PlayerPrefs.GetInt(Level3UnlockedKey, 0) == 1;
            level3Button.SetActive(isLevel3Unlocked);
            Debug.Log($"Level 3 button state: {isLevel3Unlocked}");
        }
    }

    // Method to unlock or lock Level 3 based on overall score
    public void CheckAndUnlockLevel3()
    {
        int overallScore = PlayerPrefs.GetInt(OverallScoreKey, 0); // Retrieve overall score
        string performanceCategory = GetScoreCategory(overallScore);

        if (performanceCategory == "Fair" || performanceCategory == "Excellent")
        {
            UnlockLevel3();
        }
        else
        {
            LockLevel3(); // Explicitly lock Level 3 if performance is poor
        }
    }

    private void UnlockLevel3()
    {
        PlayerPrefs.SetInt(Level3UnlockedKey, 1); // Save Level 3 unlocked state
        PlayerPrefs.SetInt(UnlockedLevelKey, 3); // Update progress to Level 3
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 3 has been unlocked!");
    }

    private void LockLevel3()
    {
        PlayerPrefs.SetInt(Level3UnlockedKey, 0); // Save Level 3 locked state
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 3 remains locked due to poor performance.");
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
