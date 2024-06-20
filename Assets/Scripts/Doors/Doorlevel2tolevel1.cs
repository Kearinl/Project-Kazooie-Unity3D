using UnityEngine;
using UnityEngine.SceneManagement;

public class Doorlevel2tolevel1 : MonoBehaviour
{
    // Scene name for the BanjoDoorIn trigger
    public string exitToSpiralMountainsScene = "Level0";

    // The game object to enable when entering the trigger zone
    public GameObject loadOutPuzzleObject;

    private bool isLoadingScene = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isLoadingScene && other.CompareTag("ExitToSpiralMountains"))
        {
            EnableLoadOutPuzzleAndLoadScene(exitToSpiralMountainsScene);
        }
    }

    private void EnableLoadOutPuzzleAndLoadScene(string sceneName)
    {
        // Stop all audio sources playing
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }

        // Enable the loadOutPuzzleObject game object
        loadOutPuzzleObject.SetActive(true);
        
        // Load the scene asynchronously
       // Resources.UnloadUnusedAssets();

        // Load the scene asynchronously
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).completed += OnSceneLoadComplete;
    }

    private void OnSceneLoadComplete(AsyncOperation asyncOperation)
    {
        isLoadingScene = false;

        // Find the player object
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Find the target object with the tag "witchtospiralmove" in the new scene
        GameObject targetObject = GameObject.FindGameObjectWithTag("witchtospiralmove");

        // If both player and target object are found, move the player to the target object's position
        if (player != null && targetObject != null)
        {
            player.transform.position = targetObject.transform.position;
        }
    }
}