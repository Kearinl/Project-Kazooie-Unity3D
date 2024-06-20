using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerObjectToggler : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public GameObject objectToEnable; // Reference to the GameObject to enable
    public GameObject objectToDisable; // Reference to the GameObject to disable

    private void Start()
    {
        // Subscribe to the videoPlayer's loopPointReached event
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // Enable the specified object and disable the other object when the video ends
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
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
