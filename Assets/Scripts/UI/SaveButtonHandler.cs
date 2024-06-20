using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SaveButtonHandler : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; // Tag of the player GameObject

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the sceneLoaded event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SavePlayerData(); // Save player data when a new scene is loaded
    }

    private void OnApplicationQuit()
    {
        SavePlayerData(); // Save player data when the application quits
    }

    public void SavePlayerData()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            Debug.Log("Player object found with tag: " + playerTag);
            PlayerData playerData = new PlayerData();
            playerData.position = playerObject.transform.position;
            Debug.Log("Player position: " + playerData.position);
            //playerData.score = 100;

            // Save the player data
            SaveLoadManager.SavePlayerData(playerData);
        }
        else
        {
            Debug.LogWarning("Player object with tag '" + playerTag + "' not found.");
        }
    }
}
