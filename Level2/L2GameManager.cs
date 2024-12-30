using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class L2GameManager : MonoBehaviour
{
    public static L2GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public GameObject cardPrefab;
    public Transform cardParent;
    public List<Sprite> cardSprites; // Full list of sprites
    public int rows = 2;
    public int columns = 4;
    public float maxTime = 60f; // Max time in seconds
    private float currentTime;

    [Header("UI Elements")]
    public Text timerText;
    public GameObject gameOverPanel;
    public Text gameOverText;
    public Button restartButton;
    public GameObject loadingScreenPanel; // Panel for loading screen

    public GameObject cardDetailsPanel; // Panel to show card details
    public Image cardDetailsImage; // Image to display the matched card's larger picture
    public Text cardDetailsText; // Text to display details of the card

    // List of card descriptions, which you can edit in the Inspector
    public List<CardDescription> cardDescriptions;

    private Card firstRevealed;
    private Card secondRevealed;
    public bool IsProcessing { get; private set; } = false;

    private int totalPairs;
    private int matchedPairs = 0;

    private Dictionary<int, int> cardPairs; // Map of sprite indices to paired indices

    private bool isTimerPaused = false; // Flag to check if the timer is paused

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateCards();
        currentTime = maxTime; // Set the current time to the max time
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        loadingScreenPanel.SetActive(false); // Hide loading screen at the start
        cardDetailsPanel.SetActive(false); // Hide the card details panel initially
    }

    private void Update()
    {
        if (currentTime > 0 && !isTimerPaused) // Only update the timer if it's not paused
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();
        }
        else if (currentTime <= 0 && !gameOverPanel.activeSelf)
        {
            ShowGameOver();
        }
    }

    private void GenerateCards()
    {
        int totalCards = rows * columns;
        int uniquePairs = totalCards / 2;

        if (cardSprites.Count < totalCards)
        {
            Debug.LogError("Not enough sprites in cardSprites! You need at least " + totalCards + " sprites.");
            return;
        }

        // Define specific pairings
        cardPairs = new Dictionary<int, int>();
        for (int i = 0; i < uniquePairs; i++)
        {
            cardPairs.Add(i * 2, i * 2 + 1); // Pair sprite 0 with 1, 2 with 3, etc.
            cardPairs.Add(i * 2 + 1, i * 2); // Reverse mapping for bidirectional lookup
        }

        // Create a shuffled list of all indices
        List<int> cardIndices = new List<int>(cardPairs.Keys);
        cardIndices.Shuffle();

        // Instantiate cards
        foreach (int cardIndex in cardIndices)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            Card card = cardObj.GetComponent<Card>();
            card.SetCard(cardIndex, cardSprites[cardIndex]);
        }

        totalPairs = uniquePairs;
    }

    public void CardRevealed(Card card)
    {
        if (firstRevealed == null)
        {
            firstRevealed = card;
        }
        else
        {
            secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        IsProcessing = true;
        yield return new WaitForSeconds(0.5f);

        if (cardPairs[firstRevealed.id] == secondRevealed.id)
        {
            Debug.Log("Match found! Pair: " + firstRevealed.id + " and " + secondRevealed.id);
            Destroy(firstRevealed.gameObject);
            Destroy(secondRevealed.gameObject);
            matchedPairs++;

            // Display the matched card in the details panel with its description
            DisplayCardDetails(firstRevealed);

            // Wait for the details panel to hide before proceeding with the level completion check
            yield return new WaitForSeconds(7f); // Wait for 7 seconds before checking if all pairs are matched

            if (matchedPairs == totalPairs)
            {
                StartCoroutine(CompleteLevel2());
            }
        }
        else
        {
            firstRevealed.FlipCard(false);
            secondRevealed.FlipCard(false);
        }

        firstRevealed = null;
        secondRevealed = null;
        IsProcessing = false;
    }

    private void DisplayCardDetails(Card card)
    {
        // Pause the timer when the card details panel opens
        isTimerPaused = true;

        // Show the details panel
        cardDetailsPanel.SetActive(true);

        // Set the larger image
        cardDetailsImage.sprite = cardSprites[card.id];

        // Find the custom description for the card
        string description = GetDescriptionForPair(card.id);

        // Display the description in the UI
        cardDetailsText.text = "Matched Pair Description: \n" + description;

        // Hide the details panel after 7 seconds
        StartCoroutine(HideCardDetailsAfterDelay());
    }

    private string GetDescriptionForPair(int cardIndex)
    {
        // Find the description for this pair from the list
        foreach (CardDescription cardDesc in cardDescriptions)
        {
            if (cardDesc.cardIndex1 == cardIndex || cardDesc.cardIndex2 == cardIndex)
            {
                return cardDesc.description;
            }
        }
        return "No description available.";
    }

    private IEnumerator HideCardDetailsAfterDelay()
    {
        yield return new WaitForSeconds(7f);
        
        // Hide the details panel
        cardDetailsPanel.SetActive(false);

        // Resume the timer after the panel is hidden
        isTimerPaused = false;
    }

    private void UpdateTimerUI()
{
    int minutes = Mathf.FloorToInt(currentTime / 60);
    int seconds = Mathf.FloorToInt(currentTime % 60);

    timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
}
    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over! Time's up.";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator CompleteLevel2()
    {
        // Wait until all card details are shown (7 seconds after the last pair is matched)
        loadingScreenPanel.SetActive(true);

        // You may want to display the level complete panel only after all card details have been revealed
        yield return new WaitForSeconds(7f); // Delay for card details to be shown

        // Mark level completion and transition to the next level
        SceneManager.LoadScene("Level1-3");
    }
}

[System.Serializable]
public class CardDescription
{
    public int cardIndex1; // The first card index in the pair
    public int cardIndex2; // The second card index in the pair
    public string description; // The description for the pair
}
