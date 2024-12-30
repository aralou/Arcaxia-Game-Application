using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreviousScene : MonoBehaviour
{
    // Start is called before the first frame update
    int sceneIndex;
    int sceneToOpen;
    void Start()
    {
        if (!PlayerPrefs.HasKey("previousScene" + sceneIndex)) { 
            PlayerPrefs.SetInt("previousScene" + sceneIndex, sceneIndex);
        }
        
        sceneToOpen = PlayerPrefs.GetInt("previousScene" + sceneIndex);
    }


    public void OnButtonClick()
    {

        SceneManager.LoadScene(sceneToOpen);
    }
}
