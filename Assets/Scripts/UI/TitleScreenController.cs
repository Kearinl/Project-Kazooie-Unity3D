using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Add this line to import the System.Collections namespace

public class TitleScreenController : MonoBehaviour
{
    // The name of the main menu scene you want to load
    public string mainMenuSceneName = "MainMenu";

    // Time in seconds to wait before allowing input to change scenes
    public float inputDelay = 10f;

    private bool inputEnabled = false;
    private bool isLoadingScene = false;
    
    // The game object to enable when entering the trigger zone
    public GameObject loadOutPuzzleObject;

    private void Start()
    {
        // Disable input initially and start the delay coroutine
        inputEnabled = false;
        StartCoroutine(EnableInputAfterDelay());
    }

    private void Update()
    {
        // Check for any button input (mouse click or keyboard input) only if input is enabled and the scene is not loading
        if (inputEnabled && !isLoadingScene && Input.anyKeyDown)
        {
            isLoadingScene = true;
            LoadMainMenuAsync();
        }
    }

    private void LoadMainMenuAsync()
    {
    
    // Enable the loadOutPuzzleObject game object
        loadOutPuzzleObject.SetActive(true);
        
        // Load the main menu scene asynchronously
        SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);
    }

    private IEnumerator EnableInputAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(inputDelay);

        // Enable input after the delay
        inputEnabled = true;
    }

    // Coroutine to wait until the main menu scene finishes loading
    private IEnumerator WaitForMainMenuLoad()
    {
        // Load the main menu scene asynchronously
        var asyncOperation = SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Single);

        // Wait until the main menu scene finishes loading
        while (!asyncOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        // Enable input after the main menu scene finishes loading
        inputEnabled = true;
    }
}
