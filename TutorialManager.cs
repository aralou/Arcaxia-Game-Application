using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI tutorialText; // Reference to the tutorial text
    private int tutorialStep = 0;

    private string[] steps = {
        "Welcome to the game! Use the joystick to move.",
        "Great! Approach the NPC to start a conversation.",
        "Complete the conversation to progress!",
        "Congratulations! You've completed the tutorial."
    };

    public Joystick joystick; // Reference to the joystick (e.g., from a Joystick script)

    private bool hasMoved = false; // Track if the player has moved to trigger step progression

    void Start()
    {
        tutorialText.text = steps[tutorialStep];
    }

    void Update()
    {
        if (tutorialStep == 0)
        {
            CheckMovementInput();
        }
    }

    private void CheckMovementInput()
    {
        // Check if the joystick is moved
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            if (!hasMoved)
            {
                hasMoved = true;
                NextStep();
            }
        }
    }

    public void NextStep()
    {
        tutorialStep++;
        if (tutorialStep < steps.Length)
        {
            tutorialText.text = steps[tutorialStep];
        }
        else
        {
            tutorialText.gameObject.SetActive(false); // Hide the tutorial
        }
    }

    // Example of triggering the next step from other scripts
    public void TriggerStepFromAction()
    {
        NextStep();
    }
}
