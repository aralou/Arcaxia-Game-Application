using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // Reference to the dialogue panel GameObject
    public GameObject dialoguePanel;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the dialogue panel is initially inactive (hidden)
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    // Method to open the dialogue panel
    public void OpenDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true); // Show the dialogue panel
        }
    }

    // Method to close the dialogue panel (optional, if you add a close button)
    public void CloseDialoguePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false); // Hide the dialogue panel
        }
    }
}
