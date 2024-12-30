using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class L3GameManager : MonoBehaviour
{
    public Image puzzleImage;
    public Text wordDisplay;
    public Button[] letterButtons;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public Text detailedFeedbackText;
    public Text secondDetailedFeedbackText;
    public Button hintButton;
    public Text hintDisplay;
    public GameObject feedbackPanel;
    public Button closeButton;
    public Text letterFeedbackText;
    public Text timerText;
    public Text scoreText;

    private string currentWord;
    private string currentAnswer;
    private int currentLevel = 0;
    private AudioSource audioSource;
    private int score = 0;
    private float timeRemaining = 30f;
    private bool isLevelActive = true;

    public string[] words = { "classroom", "back", "npc", "respect" };
    public string[] imageNames = { "classroom", "back", "npc", "respect" };
    public string[] hints = { "A place where students learn", "Opposite of front", "Non-playable character", "A core value" };
    public string[] levelFeedbackMessages = {
        "Well done! You've correctly identified the classroom!",
        "Nice! You figured out 'back'!",
        "Great job! You know what an NPC is!",
        "Awesome! Respect is one of the most important values!"
    };
    public string[] levelSecondFeedbackMessages = {
        "You really nailed it! The classroom is the heart of learning.",
        "Great job! 'Back' is such an important concept in navigation.",
        "Nice work! NPCs are everywhere in games and stories.",
        "Respect makes the world a better place! Keep it up!"
    };

    public int revealedLettersCount = 3;

    [Header("UI Elements")]
    public GameObject loadingScreenPanel;
    public Text loadingText;

  void Start()
{
    audioSource = GetComponent<AudioSource>();
    hintButton.onClick.AddListener(ShowHint);
    closeButton.onClick.AddListener(CloseFeedbackPanel);

    // Always reset score to 0 when entering this scene
    score = 0;
    scoreText.text = "Score: 0";

    timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

    // Add OnClick listeners for all letter buttons
    foreach (Button letterButton in letterButtons)
    {
        letterButton.onClick.AddListener(() => OnLetterButtonClick(letterButton));
    }

    StartLevel(currentLevel);
}
    void Update()
    {
        if (isLevelActive)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndLevel(false);
            }
        }
    }

    void StartLevel(int level)
    {
        currentWord = words[level];
        currentAnswer = new string('_', currentWord.Length);

        timeRemaining = 30f;
        isLevelActive = true;

        RevealRandomLetters();
        wordDisplay.text = AddSpacingToWord(currentAnswer);
        puzzleImage.sprite = Resources.Load<Sprite>("Images/" + imageNames[level]);

        detailedFeedbackText.text = "";
        secondDetailedFeedbackText.text = "";
        letterFeedbackText.text = "";
        hintDisplay.text = "";
        feedbackPanel.SetActive(false);
    }

    string AddSpacingToWord(string word)
    {
        char[] spacedWord = new char[word.Length * 2 - 1];
        for (int i = 0; i < word.Length; i++)
        {
            spacedWord[i * 2] = word[i];
            if (i < word.Length - 1)
            {
                spacedWord[i * 2 + 1] = ' ';
            }
        }
        return new string(spacedWord);
    }

    void RevealRandomLetters()
    {
        List<int> revealedIndices = new List<int>();

        while (revealedIndices.Count < revealedLettersCount)
        {
            int randomIndex = Random.Range(0, currentWord.Length);
            if (!revealedIndices.Contains(randomIndex))
            {
                revealedIndices.Add(randomIndex);
                currentAnswer = currentAnswer.Remove(randomIndex, 1).Insert(randomIndex, currentWord[randomIndex].ToString());
            }
        }
    }

    public void OnLetterButtonClick(Button button)
    {
        if (!isLevelActive) return;

        string selectedLetter = button.GetComponentInChildren<Text>().text;
        bool correct = false;

        for (int i = 0; i < currentWord.Length; i++)
        {
            if (currentWord[i].ToString() == selectedLetter && currentAnswer[i] == '_')
            {
                currentAnswer = currentAnswer.Remove(i, 1).Insert(i, selectedLetter);
                correct = true;
            }
        }

        wordDisplay.text = AddSpacingToWord(currentAnswer);

        if (correct)
        {
            audioSource.PlayOneShot(correctSound);
            letterFeedbackText.text = "Correct! Well done!";
        }
        else
        {
            audioSource.PlayOneShot(wrongSound);
            letterFeedbackText.text = "Incorrect! Try again!";
        }

        StartCoroutine(ClearLetterFeedbackAfterDelay(1f));

        if (!currentAnswer.Contains("_"))
        {
            if (currentLevel + 1 < words.Length)
            {
                EndLevel(true);
            }
            else
            {
                EndLevel(true);
                UnlockLevel4(); // Unlock only Level 4 when Level 3 is completed
                StartCoroutine(TransitionToMap());
            }
        }
    }

    void EndLevel(bool success)
    {
        isLevelActive = false;

        if (success)
        {
            score += 10;
            scoreText.text = "Score: " + score;

            SaveScoreToLevel6(); // Save score to Level6

            if (currentLevel < levelFeedbackMessages.Length && currentLevel < levelSecondFeedbackMessages.Length)
            {
                detailedFeedbackText.text = levelFeedbackMessages[currentLevel];
                secondDetailedFeedbackText.text = levelSecondFeedbackMessages[currentLevel];
            }
            else
            {
                detailedFeedbackText.text = "Great job!";
                secondDetailedFeedbackText.text = "You've completed the level!";
            }
            feedbackPanel.SetActive(true);

            StartCoroutine(HideFeedbackPanelAfterDelay(10f));
        }
        else
        {
            letterFeedbackText.text = "Time's up! Try again!";
            feedbackPanel.SetActive(true);
            StartCoroutine(HideFeedbackPanelAfterDelay(5f));
        }
    }

    private void UnlockLevel4()
    {
        PlayerPrefs.SetInt("Level4Unlocked", 1); // Unlock Level 4
        PlayerPrefs.Save();
    }

    private IEnumerator HideFeedbackPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackPanel.SetActive(false);

        currentLevel++;

        if (currentLevel < words.Length)
        {
            StartLevel(currentLevel);
        }
        else
        {
            UnlockLevel4(); // Ensure Level 4 unlocks at the end of Level 3
            StartCoroutine(TransitionToMap());
        }
    }

    private void SaveScoreToLevel6()
    {
        int currentLevel6Score = PlayerPrefs.GetInt("Level6OverallScore", 0);
        int updatedLevel6Score = currentLevel6Score + score;
        PlayerPrefs.SetInt("Level6OverallScore", updatedLevel6Score);
        PlayerPrefs.SetInt("Level3Score", score);
        PlayerPrefs.Save();
    }

    private IEnumerator ClearLetterFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        letterFeedbackText.text = "";
    }

    void CloseFeedbackPanel()
    {
        feedbackPanel.SetActive(false);
    }

    private IEnumerator TransitionToMap()
    {
        loadingScreenPanel.SetActive(true);
        loadingText.text = "Loading... Please Wait";
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Level1-4");
    }

    void ShowHint()
    {
        hintDisplay.text = hints[currentLevel];
    }
}
