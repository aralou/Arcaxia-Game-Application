using UnityEngine;
using UnityEngine.UI;  // For accessing UI components
using UnityEngine.SceneManagement; // For accessing scene-related functions

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance

    public AudioSource backgroundMusic;  // Reference to the AudioSource
    public Slider volumeSlider;          // Reference to the Volume Slider
    public Button muteButton;            // Reference to the Mute Button
    private bool isMuted = false;        // Mute state flag

    void Awake()
    {
        // Implement Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Keep the AudioManager alive across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instance if one already exists
        }
    }

    void Start()
    {
        // Set up the initial volume from the slider value
        backgroundMusic.volume = volumeSlider.value;

        // Add listener for the volume slider
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Add listener for the mute button
        muteButton.onClick.AddListener(ToggleMute);
    }

    // Handle the volume slider value change
    private void OnVolumeChanged(float value)
    {
        if (!isMuted)
        {
            backgroundMusic.volume = value;
        }
    }

    // Toggle mute state
    private void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            backgroundMusic.volume = 0f;  // Mute the audio
            muteButton.GetComponentInChildren<Text>().text = "Unmute";  // Change button text to "Unmute"
        }
        else
        {
            backgroundMusic.volume = volumeSlider.value;  // Set to slider value
            muteButton.GetComponentInChildren<Text>().text = "Mute";  // Change button text to "Mute"
        }
    }

    // This method can be used to load scenes while keeping the background music unchanged
    public void LoadSceneWithMusic(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
