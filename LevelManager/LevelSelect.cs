using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public Button[] levelButtons; // Assign buttons for each level in the Inspector

    void Start()
    {
        int unlockedLevel = LevelManager.Instance.UnlockedLevel;

        // Check if Level 2 should be unlocked
        if (PlayerPrefs.GetInt("Level2Unlocked", 0) == 1)
        {
            unlockedLevel = 2; // Force Level 2 as unlocked
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;

            if (levelIndex <= unlockedLevel)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].GetComponentInChildren<Text>().text = $"Level {levelIndex}";
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponentInChildren<Text>().text = $"Level {levelIndex} (Locked)";
            }
        }
    }

    private void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene($"Level{levelIndex}");
    }
}
