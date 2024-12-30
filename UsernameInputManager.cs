using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UsernameInputManager : MonoBehaviour
{
    public InputField usernameInputField; // Reference to the UI InputField
    public Text feedbackText; // Reference to the UI Text to show feedback
    public Button submitButton; // Reference to the Submit Button
    private const string UsernameKey = "Username"; // Key for storing username in PlayerPrefs
    public string nextSceneName = "NextScene"; // Name of the next scene to load

    private void Start()
    {
        // Ensure all UI references are assigned
        if (usernameInputField == null || feedbackText == null || submitButton == null)
        {
            Debug.LogError("UI references are missing! Please assign them in the Inspector.");
            return;
        }

        // Attach the SaveUsername method to the Submit Button's OnClick event
        submitButton.onClick.AddListener(SaveUsername);

        // Check for an existing username
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            string savedUsername = PlayerPrefs.GetString(UsernameKey);

            if (!string.IsNullOrEmpty(savedUsername))
            {
                feedbackText.text = $"Welcome back, {savedUsername}!";
                Debug.Log($"Welcome back message displayed for username: {savedUsername}");

                // Disable input field and submit button for existing user
                usernameInputField.interactable = false;
                submitButton.interactable = false;

                // Proceed to the next scene after a short delay
                Invoke(nameof(ProceedToNextScene), 2f);
            }
            else
            {
                Debug.LogWarning("Username key found but value is empty.");
                feedbackText.text = "Enter your username:";
            }
        }
        else
        {
            feedbackText.text = "Enter your username:";
            Debug.Log("No username found. Prompting user to create one.");
        }
    }

    public void SaveUsername()
    {
        if (usernameInputField == null || feedbackText == null)
        {
            Debug.LogError("UI references are missing! Please assign them in the Inspector.");
            return;
        }

        string username = usernameInputField.text;

        if (!string.IsNullOrEmpty(username))
        {
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save();
            feedbackText.text = $"Username saved! Welcome, {username}.";
            Debug.Log($"Username '{username}' saved successfully.");

            // Disable input field and submit button after saving
            usernameInputField.interactable = false;
            submitButton.interactable = false;

            // Proceed to the next scene after a short delay
            Invoke(nameof(ProceedToNextScene), 1f);
        }
        else
        {
            feedbackText.text = "Username cannot be empty. Please try again.";
            Debug.LogWarning("Attempted to save an empty username.");
        }
    }

    public void ClearUsername()
    {
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            PlayerPrefs.DeleteKey(UsernameKey);
            feedbackText.text = "Username cleared. Please enter a new username.";
            Debug.Log("Username cleared from PlayerPrefs.");

            // Re-enable input field and submit button
            usernameInputField.interactable = true;
            submitButton.interactable = true;
        }
        else
        {
            feedbackText.text = "No username to clear.";
            Debug.LogWarning("Attempted to clear a username, but none was found.");
        }
    }

    private void ProceedToNextScene()
    {
        // Log and load the next scene
        Debug.Log($"Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }
}
