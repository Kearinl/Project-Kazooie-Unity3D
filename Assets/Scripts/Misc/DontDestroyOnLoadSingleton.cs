using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoadSingleton : MonoBehaviour
{
    // The static instance of the script
    private static DontDestroyOnLoadSingleton instance;

    private void Awake()
    {
        // If an instance already exists, destroy this object
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // If no instance exists, set this object as the instance and mark it to not be destroyed
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Check if the current scene is "GameOver" or "MainMenu"
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "GameOver" || currentScene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }
}
