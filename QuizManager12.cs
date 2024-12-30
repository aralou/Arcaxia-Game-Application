using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Add this line
using System.Collections;
using System.Collections.Generic;

public class QuizManager12 : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers; // Array for multiple choices
        public int correctAnswerIndex; // Index of the correct answer in the answers array
        public string hint; // New field for hint per question
        public string explanation; // Explanation for the correct answer
    }

    public Text questionText;
    public Button[] answerButtons;
    public Text scoreText;
    public Text timerText;
    public Text hintText;
    public Text explanationText; // Text to display explanation
    public Button hintButton;
    public Button nextButton; // Button to move to next question after an explanation

    public List<Question> questions;
    private Question currentQuestion;
    private int currentQuestionIndex;
    private int score;
    private float timer;

    public float timePerQuestion = 10f;

    [Header("UI Elements")]
    public GameObject loadingScreenPanel;
    public Text loadingText;

    private bool levelCompleted = false; // To ensure Level 4 is marked only once

    private void Start()
    {
        // Reset the score to 0 every time a new scene is loaded
        score = 0;
        scoreText.text = "Score: " + score;

        timer = timePerQuestion;
        currentQuestionIndex = 0;

        // Add listener for the Hint Button
        hintButton.onClick.AddListener(ShowHint);

        // Add listener for the Next Button
        nextButton.onClick.AddListener(LoadNextQuestion);

        LoadNextQuestion();
    }

    private void Update()
    {
        if (timer > 0 && explanationText.text == "")
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.Max(timer, 0).ToString("F1");
        }

        if (timer <= 0 && explanationText.text == "")
        {
            LoadNextQuestion();
        }
    }

    public void LoadNextQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        timer = timePerQuestion;

        currentQuestion = questions[currentQuestionIndex];
        currentQuestionIndex++;

        questionText.text = currentQuestion.questionText;
        hintText.text = ""; // Clear hint text
        explanationText.text = ""; // Clear explanation text
        hintButton.interactable = true; // Enable hint button
        nextButton.gameObject.SetActive(false); // Hide the next button initially

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<Text>().text = currentQuestion.answers[i];

                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnAnswerSelected(int index)
    {
        if (index == currentQuestion.correctAnswerIndex)
        {
            // Award 10 points for each correct answer
            score += 10;
            scoreText.text = "Score: " + score;

            // Save the updated score immediately
            SaveScoreToLevel6();

            LoadNextQuestion();
        }
        else
        {
            // Deduct 5 points for a wrong answer
            score -= 5;
            if (score < 0) score = 0; // Prevent score from going below 0

            scoreText.text = "Score: " + score;

            explanationText.text = "Wrong Answer! The correct answer is: " + currentQuestion.answers[currentQuestion.correctAnswerIndex] +
                                   "\nExplanation: " + currentQuestion.explanation;

            nextButton.gameObject.SetActive(true);

            // Save the updated score immediately
            SaveScoreToLevel6();
        }
    }

    private void ShowHint()
    {
        hintText.text = "Hint: " + currentQuestion.hint;
        hintButton.interactable = false;
    }

    private void EndGame()
    {
        // Save the final score when the level ends
        SaveScoreToLevel6();

        if (!levelCompleted) // Ensure Level 4 is marked as completed only once
        {
            levelCompleted = true;
        }

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        StartCoroutine(TransitionToMap());
    }

    private void SaveScoreToLevel6()
    {
        int currentLevel6Score = PlayerPrefs.GetInt("Level6OverallScore", 0);
        int updatedLevel6Score = currentLevel6Score + score;

        PlayerPrefs.SetInt("Level6OverallScore", updatedLevel6Score);
        PlayerPrefs.SetInt("Level4Score", score);
        PlayerPrefs.Save();
    }

    private IEnumerator TransitionToMap()
    {
        loadingScreenPanel.SetActive(true);
        loadingText.text = "Loading... Please Wait";

        yield return new WaitForSeconds(10f);

        SceneManager.LoadScene("Level2-4");
    }
}
