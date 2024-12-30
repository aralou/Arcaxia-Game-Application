using UnityEngine;

public class ShowButtonAfterDelay : MonoBehaviour
{
    // Assign the button GameObject in the Unity Inspector
    [SerializeField] private GameObject button;

    // Delay in seconds
    [SerializeField] private float delay = 10f;

    private void Start()
    {
        if (button != null)
        {
            button.SetActive(false); // Hide the button initially
            Invoke(nameof(ShowButton), delay); // Call ShowButton after the delay
        }
        else
        {
            Debug.LogWarning("Button reference is missing!");
        }
    }

    // Function to make the button visible
    private void ShowButton()
    {
        button.SetActive(true);
    }
}
