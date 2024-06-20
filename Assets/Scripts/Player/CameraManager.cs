using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Reference to the main camera
    private Camera mainCamera;

    // Initial camera settings (you can adjust these based on your needs)
    public float initialFieldOfView = 60f;
    public Color initialBackgroundColor = Color.black;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Call this method to reset the camera's settings before loading a new scene
    public void ResetCamera()
    {
        // Reset camera field of view
        mainCamera.fieldOfView = initialFieldOfView;

        // Reset camera background color
        mainCamera.backgroundColor = initialBackgroundColor;
    }
}
