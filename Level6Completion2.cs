using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level6Completion2 : MonoBehaviour
{
    public Button unlockLevel3Button;

    private void Start()
    {
        unlockLevel3Button.onClick.AddListener(UnlockLevel3);
    }

    private void UnlockLevel3()
    {
        PlayerPrefs.SetInt("Level3Unlocked", 1); // Save the unlock flag
        PlayerPrefs.Save();
        Debug.Log("Level 3 is now unlocked!");
        SceneManager.LoadScene("Map"); // Transition to the map scene
    }
}
