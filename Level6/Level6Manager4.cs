using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level6Manager4 : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject choicePanel;         // UI panel for choices
    public Text overallScoreText;         // UI Text to display the overall score
    public Text performanceText;          // UI Text to show Poor/Fair/Excellent
    public GameObject certificatePanel;   // UI panel to display the certificate
    public Image certificateImage;        // UI Image to display the certificate
    public Sprite certificateSprite;      // Certificate sprite to be displayed
    public Text usernameText;             // UI Text to display the username on the certificate
    public Button goToMapButton;          // Button for going to the map
    public Button replayButton;           // Button for replaying the game

    private const string UsernameKey = "Username";

    private void Start()
    {
        // Display the overall score
        DisplayOverallScore();

        // Show the certificate first, then the choice panel
        if (SceneManager.GetActiveScene().name == "Level4-5")
        {
            ShowCertificate();
        }
        else
        {
            Debug.LogWarning($"Current Scene is NOT Level4-5: {SceneManager.GetActiveScene().name}");
        }
    }

    private void ShowCertificate()
    {
        string username = PlayerPrefs.GetString(UsernameKey, "Player");

        if (certificatePanel != null && certificateImage != null && certificateSprite != null)
        {
            certificateImage.sprite = certificateSprite;

            if (usernameText != null)
            {
                usernameText.text = $"Congratulations, {username}!";
            }

            certificatePanel.SetActive(true);
            Invoke(nameof(ShowChoicePanel), 10f);
        }
        else
        {
            Debug.LogError("Certificate panel, image, or sprite is not assigned in the inspector.");
            ShowChoicePanel();
        }
    }

    private void ShowChoicePanel()
    {
        if (certificatePanel != null) certificatePanel.SetActive(false);

        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
            Time.timeScale = 0; // Pause the game
        }
        else
        {
            Debug.LogError("Choice Panel is not assigned in the inspector.");
        }
    }

    private void DisplayOverallScore()
    {
        int level1Score = PlayerPrefs.GetInt("Level1Score", 0);
        int level2Score = PlayerPrefs.GetInt("Level2Score", 0);
        int level3Score = PlayerPrefs.GetInt("Level3Score", 0);
        int level4Score = PlayerPrefs.GetInt("Level4Score", 0);
        int level5Score = PlayerPrefs.GetInt("Level5Score", 0);

        int overallScore = level1Score + level2Score + level3Score + level4Score + level5Score;

        PlayerPrefs.SetInt("OverallScore", overallScore);
        PlayerPrefs.Save();

        if (overallScoreText != null)
        {
            overallScoreText.text = "Overall Score: " + overallScore;
        }

        DisplayPerformance(overallScore); // Determine and display performance category
    }

    private void DisplayPerformance(int score)
    {
        string performanceCategory = GetScoreCategory(score);

        if (performanceText != null)
        {
            performanceText.text = $"Performance: {performanceCategory}";
        }

        AdjustButtonVisibility(performanceCategory);

        // Check and unlock Level 3 based on performance
        LevelManager3.Instance?.CheckAndUnlockLevel4();

        Debug.Log($"Player Performance Category: {performanceCategory}");
    }

    private string GetScoreCategory(int finalScore)
    {
        if (finalScore <= 80) return "Poor";
        else if (finalScore <= 160) return "Fair";
        else return "Excellent";
    }

    private void AdjustButtonVisibility(string performanceCategory)
    {
        if (performanceCategory == "Poor")
        {
            if (goToMapButton != null) goToMapButton.gameObject.SetActive(false);
            if (replayButton != null) replayButton.gameObject.SetActive(true);
        }
        else
        {
            if (goToMapButton != null) goToMapButton.gameObject.SetActive(true);
            if (replayButton != null) replayButton.gameObject.SetActive(false);
        }
    }

    public void OnGoToMapButtonClicked()
    {
        Time.timeScale = 1; // Unpause the game
        SceneManager.LoadScene("Map");
    }

    public void OnReplayButtonClicked()
    {
        ResetGameState();
        Time.timeScale = 1; // Unpause the game
        SceneManager.LoadScene("Level4");
    }

    public void OnCloseAppButtonClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ResetGameState()
    {
        // Reset level scores and lock Level 4
        PlayerPrefs.DeleteKey("Level1Score");
        PlayerPrefs.DeleteKey("Level2Score");
        PlayerPrefs.DeleteKey("Level3Score");
        PlayerPrefs.DeleteKey("Level4Score");
        PlayerPrefs.DeleteKey("Level5Score");
        PlayerPrefs.DeleteKey("OverallScore");
        PlayerPrefs.SetInt("Level5Unlocked", 0); // Lock Level 4
        PlayerPrefs.Save();

        Debug.Log("Game state reset. Level 5 locked.");
    }
}
