using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC4 : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue; // Main dialogue array
    private int index = 0;

    // Custom responses for Yes/No
    public string[] yesResponses; // Custom "Yes" responses
    public string[] noResponses;  // Custom "No" responses

    public GameObject yesButton;
    public GameObject noButton;
    public Slider progressBar;
    public TextMeshProUGUI progressText; // Optional for showing percentages
    public TextMeshProUGUI timerText;    // Text to display the countdown timer
    public float wordSpeed;
    public bool playerIsClose;

    public GameManager4 gameManager4; // Reference to the GameManager

    private bool hasInteracted = false; // Tracks if interaction has already occurred
    private Coroutine typingCoroutine; // Reference to the coroutine for stopping/resuming typing
    private Coroutine answerTimeoutCoroutine; // Reference for the 30-second timeout coroutine

    // Reference to the new panel (set in the Inspector)
    public GameObject newPanel;

    // The GameObject to hide after interaction
    public GameObject objectToHide;

    private const float answerTimeout = 30f; // Time in seconds to wait for an answer
    private float remainingTime; // Remaining time for the timer

    // Define points for positive and negative responses
    public int positivePoints = 10;
    public int negativePoints = -5;

    // Hint system variables
    public string hint; // Hint specific to this NPC
    public GameObject hintButton; // Hint button reference
    public TextMeshProUGUI hintText; // Text element to display the hint

    void Start()
    {
        dialogueText.text = "";
        timerText.text = ""; // Ensure the timer text is empty initially

        // Ensure Yes/No buttons are hidden initially
        if (yesButton != null) yesButton.SetActive(false);
        if (noButton != null) noButton.SetActive(false);

        // Set up Yes/No button onClick listeners
        if (yesButton != null)
        {
            yesButton.GetComponent<Button>().onClick.AddListener(OnYesButtonClicked);
        }

        if (noButton != null)
        {
            noButton.GetComponent<Button>().onClick.AddListener(OnNoButtonClicked);
        }

        // Set up Hint Button functionality
        if (hintButton != null)
        {
            hintButton.SetActive(true); // Ensure the button is visible
            hintButton.GetComponent<Button>().onClick.AddListener(DisplayHint);
        }

        // Initialize the progress bar
        UpdateProgressBar();
    }

    public void DisplayHint()
    {
        // Show the hint in the UI
        if (hintText != null)
        {
            hintText.text = hint; // Display the hint
        }
    }

    public void RemoveText()
    {
        // Ensure dialoguePanel exists before accessing it
        if (dialoguePanel != null)
        {
            dialogueText.text = "";
            index = 0;
            dialoguePanel.SetActive(false);
        }

        // Check if buttons are still assigned before disabling them
        if (yesButton != null) yesButton.SetActive(false);
        if (noButton != null) noButton.SetActive(false);

        StopAnswerTimeout(); // Stop the timeout when dialogue ends
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        if (IsQuestion(dialogue[index]))
        {
            ShowQuestionButtons();
            StartAnswerTimeout(); // Start the 30-second timeout
        }
        else
        {
            yield return new WaitForSeconds(1f);
            NextLine();
        }
    }

    private bool IsQuestion(string line)
    {
        return line.EndsWith("?");
    }

    private void ShowQuestionButtons()
    {
        if (yesButton != null) yesButton.SetActive(true);
        if (noButton != null) noButton.SetActive(true);
    }

    public void NextLine()
    {
        if (yesButton != null) yesButton.SetActive(false);
        if (noButton != null) noButton.SetActive(false);

        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            typingCoroutine = StartCoroutine(Typing());
        }
        else
        {
            EndInteraction();
        }
    }

    public void OnYesButtonClicked()
    {
        StopAnswerTimeout(); // Stop the timeout on interaction

        // Hide buttons immediately
        if (yesButton != null) yesButton.SetActive(false);
        if (noButton != null) noButton.SetActive(false);

        // Add positive points to the total score
        ScoreManager.AddScore(positivePoints);
        UpdateProgressBar();

        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // Deactivate the GameObject
        }

        if (yesResponses != null && yesResponses.Length > index)
        {
            dialogueText.text = yesResponses[index];
        }
        else
        {
            dialogueText.text = "You answered positively!";
        }

        Invoke(nameof(NextLine), 5f);
    }

    public void OnNoButtonClicked()
    {
        StopAnswerTimeout(); // Stop the timeout on interaction

        // Hide buttons immediately
        if (yesButton != null) yesButton.SetActive(false);
        if (noButton != null) noButton.SetActive(false);

        // Subtract negative points from the total score
        ScoreManager.AddScore(negativePoints);
        UpdateProgressBar();

        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // Deactivate the GameObject
        }

        if (noResponses != null && noResponses.Length > index)
        {
            dialogueText.text = noResponses[index];
        }
        else
        {
            dialogueText.text = "You answered negatively!";
        }

        Invoke(nameof(HideDialoguePanel), 5f);
        Invoke(nameof(ShowNewPanel), 5f);

        hasInteracted = true;

        if (gameManager4 != null) gameManager4.NPCFinished();  // Notify GameManager that interaction is complete
    }

    private void HideDialoguePanel()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void ShowNewPanel()
    {
        if (newPanel != null)
        {
            newPanel.SetActive(true);
        }
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            // Optionally update based on score or interaction type
            progressBar.value = Mathf.InverseLerp(-100, 100, ScoreManager.GetTotalScore()); // Normalize the score for the progress bar
        }

        if (progressText != null)
        {
            progressText.text = $"Score: {ScoreManager.GetTotalScore()}";
        }
    }

    private void EndInteraction()
    {
        RemoveText();
        hasInteracted = true;

        if (objectToHide != null)
        {
            objectToHide.SetActive(false); // Deactivate the GameObject
        }

        if (gameManager4 != null) gameManager4.NPCFinished();  // Notify GameManager that interaction is complete
    }

    private void StartAnswerTimeout()
    {
        StopAnswerTimeout(); // Stop any existing timeout to prevent overlap
        remainingTime = answerTimeout;
        answerTimeoutCoroutine = StartCoroutine(AnswerTimeout());
    }

    private void StopAnswerTimeout()
    {
        if (answerTimeoutCoroutine != null)
        {
            StopCoroutine(answerTimeoutCoroutine);
            answerTimeoutCoroutine = null;
        }

        if (timerText != null) timerText.text = ""; // Clear the timer text
    }

    private IEnumerator AnswerTimeout()
    {
        while (remainingTime > 0)
        {
            if (timerText != null)
            {
                timerText.text = $"Time left: {Mathf.CeilToInt(remainingTime)}s"; // Update the timer text
            }

            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        // If the player hasn't answered within the timeout, close the dialogue
        if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            dialogueText.text = "You took too long to answer!";
            if (timerText != null) timerText.text = ""; // Clear the timer text
            Invoke(nameof(RemoveText), 5f); // Close dialogue after showing timeout message
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            playerIsClose = true;

            if (dialoguePanel != null && !dialoguePanel.activeInHierarchy)
            {
                dialoguePanel.SetActive(true);
                typingCoroutine = StartCoroutine(Typing());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;

            // Ensure the typing coroutine is stopped if still active
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Check if dialoguePanel exists before trying to deactivate it
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
}