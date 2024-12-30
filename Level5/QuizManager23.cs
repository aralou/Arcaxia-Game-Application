using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class QuizManager23 : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers; // Array for multiple choices
        public int correctAnswerIndex; // Index of the correct answer in the answers array
    }

    public Text questionText;
    public Button[] answerButtons;
    public Text scoreText;
    public Text timerText;

    public List<Question> questions;
    private Question currentQuestion;
    private int currentQuestionIndex;
    private int score;

    private float timeRemaining = 20f;
    private bool isQuestionActive = true;

    [Header("UI Elements")]
    public GameObject loadingScreenPanel;
    public Text loadingText;

    private void Start()
    {
        score = 0;
        currentQuestionIndex = 0;
        timeRemaining = 20f;
        isQuestionActive = true;

        scoreText.text = "Score: 0";
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

        LoadNextQuestion();
    }

    private void Update()
    {
        if (isQuestionActive)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                ProceedToNextQuestion();
            }
        }
    }

    public void LoadNextQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        currentQuestion = questions[currentQuestionIndex];
        currentQuestionIndex++;
        isQuestionActive = true;
        timeRemaining = 20f;

        questionText.text = currentQuestion.questionText;

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
        if (!isQuestionActive) return;

        isQuestionActive = false;

        if (index == currentQuestion.correctAnswerIndex)
        {
            score += 10;
            scoreText.text = "Score: " + score;
        }

        StartCoroutine(LoadNextQuestionAfterDelay(1f));
    }

    private void ProceedToNextQuestion()
    {
        isQuestionActive = false;
        StartCoroutine(LoadNextQuestionAfterDelay(1f));
    }

    private IEnumerator LoadNextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextQuestion();
    }

    private void EndGame()
    {
        isQuestionActive = false;
        questionText.text = "Quiz Complete! Final Score: " + score;

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        PlayerPrefs.SetInt("Level5Unlocked", 1);
        PlayerPrefs.Save();



        // Save the Level 5 score to the overall Level 6 score
        SaveScoreToLevel6();

        StartCoroutine(TransitionToLevel6());
    }

    private void SaveScoreToLevel6()
    {
        // Get the current Level 6 overall score, if exists
        int currentLevel6Score = PlayerPrefs.GetInt("Level6OverallScore", 0);

        // Add the Level 5 score to the Level 6 overall score
        int updatedLevel6Score = currentLevel6Score + score;

        // Save the new overall score in PlayerPrefs
        PlayerPrefs.SetInt("Level6OverallScore", updatedLevel6Score);

        // Optionally, also save the Level 5 score separately
        PlayerPrefs.SetInt("Level5Score", score);

        PlayerPrefs.Save(); // Save all changes
    }

    private IEnumerator TransitionToLevel6()
    {
        loadingScreenPanel.SetActive(true);
        loadingText.text = "Loading... Please wait";

        yield return new WaitForSeconds(10f);

        // After the delay, load the "Level2-5" scene
        SceneManager.LoadScene("Level3-5");
    }
}
