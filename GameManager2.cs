using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager2 : MonoBehaviour
{
    public NPC2[] npcs;  // Array of all NPCs in the scene
    private int npcCount; // Keep track of remaining NPCs

    public GameObject loadingScreen;  // Reference to the loading screen GameObject
    public Slider loadingBar;         // Optional: If you want to show progress with a loading bar
    public TextMeshProUGUI npcCounterText; // Text to display the remaining NPC count

    public GameObject levelMessagePanel;  // Reference to the level completion message panel
    public TextMeshProUGUI levelMessageText; // Reference to the text on the panel

    public GameObject dialoguePanel;  // Reference to the dialogue panel
    private bool isDialogueActive = false; // Tracks if the dialogue panel is active

    public int currentLevel = 1;
    public int maxLevels = 3;
    public int npcsPerLevel = 2; // Number of NPCs required to complete a level
    private int interactedNPCs = 0;

    public float loadingDelay = 6f;   // Delay before starting the next scene load

    void Start()
    {
        // Reset score when entering this scene
        ScoreManager.ResetScore();
        Debug.Log("Score reset to 0 on entering this scene.");

        // Count the number of NPCs in the scene at the start
        npcCount = npcs.Length;

        // Update the NPC counter UI initially
        UpdateNPCCounter();

        // Initialize the level message panel
        if (levelMessagePanel != null)
        {
            levelMessagePanel.SetActive(false); // Ensure the panel is hidden at start
        }

        // Initialize the dialogue panel state
        if (dialoguePanel != null)
        {
            isDialogueActive = dialoguePanel.activeSelf;
        }
    }

    void Update()
    {
        // Check the state of the dialogue panel in each frame
        if (dialoguePanel != null)
        {
            isDialogueActive = dialoguePanel.activeSelf;
        }
    }

    // Called when an NPC finishes its interaction
    public void NPCFinished()
    {
        npcCount--;  // Decrement the NPC count
        interactedNPCs++; // Increment interacted NPCs for the current level

        // Update the NPC counter UI after an NPC interaction is finished
        UpdateNPCCounter();

        // Check if the current level is completed
        if (interactedNPCs >= npcsPerLevel)
        {
            StartCoroutine(WaitForDialogueThenCompleteLevel());
        }
    }

    // Method to update the NPC counter text
    private void UpdateNPCCounter()
    {
        if (npcCounterText != null)
        {
            npcCounterText.text = $"NPCs Left: {npcCount}"; // Display remaining NPC count
        }
    }

    // Coroutine to wait for the dialogue panel to close before showing the level completion message
    private IEnumerator WaitForDialogueThenCompleteLevel()
    {
        // Wait until the dialogue panel is inactive
        while (isDialogueActive)
        {
            yield return null;
        }

        // Proceed to complete the level
        CompleteLevel();
    }

    private void CompleteLevel()
    {
        string customLevelMessage = "";
        switch (currentLevel)
        {
            case 1:
                customLevelMessage = "Easy Mode Completed!";
                break;
            case 2:
                customLevelMessage = "Medium Mode Completed!";
                break;
            case 3:
                customLevelMessage = "Hard Mode Completed!";
                break;
            default:
                customLevelMessage = "Level Completed!";
                break;
        }

        if (levelMessagePanel != null && levelMessageText != null)
        {
            StartCoroutine(ShowLevelMessageAfterDialogue(customLevelMessage));
        }
    }

    private IEnumerator ShowLevelMessageAfterDialogue(string customLevelMessage)
    {
        while (isDialogueActive)
        {
            yield return null;
        }

        levelMessageText.text = customLevelMessage;
        levelMessagePanel.SetActive(true);

        StartCoroutine(HideLevelMessage());
    }

    private IEnumerator HideLevelMessage()
    {
        yield return new WaitForSeconds(5f);
        if (levelMessagePanel != null)
        {
            levelMessagePanel.SetActive(false);
        }

        AdvanceLevelOrComplete();
    }

    private void AdvanceLevelOrComplete()
    {
        if (currentLevel < maxLevels)
        {
            currentLevel++;
            interactedNPCs = 0;
        }
        else
        {
            if (npcCount <= 0)
            {
                StartCoroutine(LevelComplete());
            }
        }
    }

    private IEnumerator LevelComplete()
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(loadingDelay);

        PlayerPrefs.SetInt($"Level{currentLevel}Completed", 1);
        PlayerPrefs.Save();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level2-2");

        while (!asyncOperation.isDone)
        {
            if (loadingBar != null)
            {
                loadingBar.value = asyncOperation.progress;
            }

            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
