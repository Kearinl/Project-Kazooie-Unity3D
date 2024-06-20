using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuButton : MonoBehaviour
{
    // Reference to the CameraManager GameObject
    public CameraManager cameraManager;

    public void LoadMainMenu()
    {
        Debug.Log("LoadMainMenu: Started");

        // Unload unused assets to free up memory
        //Resources.UnloadUnusedAssets();

        // Reset camera settings before loading the "MainMenu" scene
        cameraManager.ResetCamera();

        // Stop all audio sources playing
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();
        }

        // Load the "MainMenu" scene asynchronously and handle scene load
        SceneManager.sceneLoaded += OnMainMenuSceneLoaded;
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);

        Debug.Log("LoadMainMenu: Loading scene...");
    }

    private void OnMainMenuSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnMainMenuSceneLoaded: Scene loaded");

        // Scene is loaded, now perform any necessary initialization
        // For example, trigger animations or activate objects here

        // Remove the callback to prevent it from being called again
        SceneManager.sceneLoaded -= OnMainMenuSceneLoaded;

        Debug.Log("OnMainMenuSceneLoaded: Callback removed");
    }
}
