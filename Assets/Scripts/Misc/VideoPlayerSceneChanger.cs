using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoPlayerSceneChanger : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    private void Start()
    {
        // Subscribe to the videoPlayer's loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Load the MainMenu scene when the video ends
       SceneManager.LoadSceneAsync("MainMenu");
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
