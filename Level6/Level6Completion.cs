using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level6Completion : MonoBehaviour
{
    public Button unlockLevel2Button;

    private void Start()
    {
        unlockLevel2Button.onClick.AddListener(UnlockLevel2);
    }

    private void UnlockLevel2()
    {
        PlayerPrefs.SetInt("Level2Unlocked", 1); // Save the unlock flag
        PlayerPrefs.Save();
        Debug.Log("Level 2 is now unlocked!");
        SceneManager.LoadScene("Map"); // Transition to the map scene
    }
}
