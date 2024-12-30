using UnityEngine;
using UnityEngine.UI;

public class LevelManager3 : MonoBehaviour
{
    public static LevelManager3 Instance; // Singleton instance specific to LevelManager2

    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string Level4UnlockedKey = "Level4Unlocked";
    private const string OverallScoreKey = "OverallScore";

    public int UnlockedLevel { get; private set; } = 1; // Default to Level 1 being unlocked
    public GameObject level4Button; // Reference to Level 4 button in the map scene

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
        if (level4Button != null)
        {
            bool isLevel4Unlocked = PlayerPrefs.GetInt(Level4UnlockedKey, 0) == 1;
            level4Button.SetActive(isLevel4Unlocked);
            Debug.Log($"Level 4 button state: {isLevel4Unlocked}");
        }
    }

    // Method to unlock or lock Level 3 based on overall score
    public void CheckAndUnlockLevel4()
    {
        int overallScore = PlayerPrefs.GetInt(OverallScoreKey, 0); // Retrieve overall score
        string performanceCategory = GetScoreCategory(overallScore);

        if (performanceCategory == "Fair" || performanceCategory == "Excellent")
        {
            UnlockLevel4();
        }
        else
        {
            LockLevel4(); // Explicitly lock Level 3 if performance is poor
        }
    }

    private void UnlockLevel4()
    {
        PlayerPrefs.SetInt(Level4UnlockedKey, 1); // Save Level 3 unlocked state
        PlayerPrefs.SetInt(UnlockedLevelKey, 4); // Update progress to Level 3
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 4 has been unlocked!");
    }

    private void LockLevel4()
    {
        PlayerPrefs.SetInt(Level4UnlockedKey, 0); // Save Level 3 locked state
        PlayerPrefs.Save();
        UpdateLevelButtons();
        Debug.Log("Level 4 remains locked due to poor performance.");
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
