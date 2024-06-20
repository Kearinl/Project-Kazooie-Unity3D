using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerSceneChangernew : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public GameObject objectToDisable; // Reference to the GameObject to disable
    public GameObject objectToEnable; // Reference to the GameObject to enable
    public GameObject objectToEnable1; // Reference to the first GameObject to enable
    public GameObject objectToEnable2; // Reference to the second GameObject to enable
    public GameObject enviroEffects; // Reference to the Enviro effects GameObject

    private bool videoEnded = false; // Flag to track if the video ended naturally

    private void Start()
    {
        // Subscribe to the videoPlayer's loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;

        // Disable Enviro effects when the video starts playing
        DisableEnviroEffects();
    }

    private void Update()
    {
        // Check for any button press
        if (Input.anyKeyDown)
        {
            videoEnded = false; // Video did not end naturally
            HandleGameObjects();
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Set the flag indicating the video ended naturally
        videoEnded = true;

        // Handle the GameObjects when the video ends
        HandleGameObjects();
    }

    private void HandleGameObjects()
    {
        // Disable the referenced GameObject
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        // Enable the referenced GameObjects
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
        if (objectToEnable1 != null)
        {
            objectToEnable1.SetActive(true);
        }
        if (objectToEnable2 != null)
        {
            objectToEnable2.SetActive(true);
        }

        // Re-enable Enviro effects
        EnableEnviroEffects();
    }

    private void DisableEnviroEffects()
    {
        if (enviroEffects != null)
        {
            enviroEffects.SetActive(false);
        }
    }

    private void EnableEnviroEffects()
    {
        if (enviroEffects != null)
        {
            enviroEffects.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the videoPlayer's loopPointReached event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
