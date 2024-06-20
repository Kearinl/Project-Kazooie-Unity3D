using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject playerBanjo;
    public GameObject[] banjoCameras; // Array to hold multiple BanjoCameras
    private bool isPaused = false;
    private Vector3 playerBanjoPosition;

    private void Start()
    {
        // Make sure the pause menu is initially disabled
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Get the initial position of the PlayerBanjo GameObject
        playerBanjoPosition = playerBanjo.transform.position;
    }

    private void Update()
    {
        // Check for the Escape key press to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        // Enable the pause menu UI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        // Disable the playerBanjo GameObject
        if (playerBanjo != null)
        {
            playerBanjo.SetActive(false);
        }

        // Disable the BanjoCameras
        foreach (var camera in banjoCameras)
        {
            if (camera != null)
            {
                camera.SetActive(false);
            }
        }

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Set the time scale to 0 to stop all gameplay mechanics
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        // Disable the pause menu UI
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Re-enable the playerBanjo GameObject
        if (playerBanjo != null)
        {
            playerBanjo.SetActive(true);
            // Reset the position of the playerBanjo GameObject to its initial position
            playerBanjo.transform.position = playerBanjoPosition;
        }

        // Re-enable the BanjoCameras
        foreach (var camera in banjoCameras)
        {
            if (camera != null)
            {
                camera.SetActive(true);
            }
        }

        // Unlock the cursor and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the time scale back to 1 to resume normal gameplay mechanics
        Time.timeScale = 1f;
        isPaused = false;
    }

    // Call this method from the "ReturnToGameButton" OnClick event in your canvas
    public void OnReturnToGameButtonClick()
    {
        ResumeGame();
    }
}
