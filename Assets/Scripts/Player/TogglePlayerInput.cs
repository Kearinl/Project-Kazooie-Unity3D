using UnityEngine;
using UnityEngine.InputSystem;

public class TogglePlayerInput : MonoBehaviour
{
    public GameObject[] players;
    private bool isInputEnabled = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isInputEnabled = !isInputEnabled;
            TogglePlayerInputComponent(isInputEnabled);
        }
    }

    private void TogglePlayerInputComponent(bool enable)
    {
        foreach (GameObject player in players)
        {
            // Get the PlayerInput component from the specific player GameObject
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = enable;
            }
        }
    }
}