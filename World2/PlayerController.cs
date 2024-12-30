using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    void Start()
    {
        // Check if there is a saved position, and if so, teleport the player there
        if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY"))
        {
            float posX = PlayerPrefs.GetFloat("PlayerPosX");
            float posY = PlayerPrefs.GetFloat("PlayerPosY");
            transform.position = new Vector2(posX, posY);
        }
        else
        {
            // Default spawn point if no saved position exists
            transform.position = new Vector2(0, 0);
        }
    }
}
