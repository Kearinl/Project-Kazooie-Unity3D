using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform player;   // Reference to the player's Transform
    public Vector3 offset;     // Offset between the camera and player
    public float sensitivity = 2.0f;   // Mouse look sensitivity
    public float rotationSpeed = 100.0f;   // Gamepad rotation speed

    private Vector3 velocity = Vector3.zero;
    private float gamepadRotation = 10.0f;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    void LateUpdate()
    {
        // Camera follows the player with a smooth dampening effect
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);

        // Player rotation based on input
        float lookInput = _playerInput.actions["Look"].ReadValue<Vector2>().x;
        transform.Rotate(Vector3.up * lookInput * sensitivity * Time.deltaTime);

        // Gamepad rotation
        if (Gamepad.current != null)
        {
            float rotationInput = _playerInput.actions["Look"].ReadValue<Vector2>().x;
            gamepadRotation = Mathf.Lerp(gamepadRotation, rotationInput, rotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up * gamepadRotation * sensitivity * Time.deltaTime);
        }
    }
}
