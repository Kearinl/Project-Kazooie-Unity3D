using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    public string level1SceneName = "Level1";
    public UnityEngine.UI.Button newGameButton;
    public UnityEngine.UI.Button continueButton; // Reference to the continue button
    private bool inputEnabled = false;
    public GameObject loadOutPuzzleObject;
    public GameObject objectToDisable; // Reference to the GameObject to disable
    public GameObject loadingScreen; // Reference to a loading screen object

    private void Start()
    {
        inputEnabled = false;
        StartCoroutine(EnableInputAfterDelay());
        newGameButton.onClick.AddListener(OnNewGameButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked); // Add listener for the continue button
    }

    private void OnNewGameButtonClicked()
    {
        if (inputEnabled)
        {
            StartCoroutine(PrepareAndLoadLevel1(true));
        }
    }

    private void OnContinueButtonClicked()
    {
        if (inputEnabled)
        {
            StartCoroutine(PrepareAndLoadLevel1(false));
        }
    }

    private IEnumerator PrepareAndLoadLevel1(bool deleteFiles)
    {
        Debug.Log("Starting PrepareAndLoadLevel1 coroutine");

        // Show loading screen
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        // Disable the referenced game object
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        // Enable the loadOutPuzzleObject game object
        if (loadOutPuzzleObject != null)
        {
            loadOutPuzzleObject.SetActive(true);
        }

        if (deleteFiles)
        {
            Debug.Log("Deleting game files...");
            Task deleteFilesTask = Task.Run(() => DeleteGameFiles());
            yield return new WaitUntil(() => deleteFilesTask.IsCompleted);
        }

        yield return LoadLevel1Async();
    }

    private void DeleteGameFiles()
    {
        // Delete all .game files in the directory
        string directoryPath = Application.persistentDataPath;
        string[] gameFiles = Directory.GetFiles(directoryPath, "*.game");
        foreach (string filePath in gameFiles)
        {
            File.Delete(filePath);
        }
    }

    private IEnumerator LoadLevel1Async()
    {
        Debug.Log("Starting LoadLevel1Async coroutine");

        // Load the level 1 scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level1SceneName, LoadSceneMode.Single);

        // While the asynchronous scene loads, display a loading screen or progress bar if needed
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Scene loaded successfully");

        // Hide the loading screen after the scene is loaded
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }

    private IEnumerator EnableInputAfterDelay()
    {
        Debug.Log("Starting EnableInputAfterDelay coroutine");
        yield return new WaitForSeconds(0.5f);
        inputEnabled = true;
        Debug.Log("Input enabled");
    }
}

