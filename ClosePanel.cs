using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    // Assign this in the Unity Inspector
    [SerializeField] private GameObject panel;

    // Function to hide the panel
    public void Close()
    {
        if (panel != null)
        {
            panel.SetActive(false); // Deactivates the panel
        }
        else
        {
            Debug.LogWarning("Panel reference is missing!");
        }
    }
}
