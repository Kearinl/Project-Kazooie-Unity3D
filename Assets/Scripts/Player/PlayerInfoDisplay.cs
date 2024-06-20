using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour
{
    public string playerTag = "Player"; // The tag assigned to the player GameObject
    public Text playerInfoText;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform != null && playerInfoText != null)
        {
            Vector3 position = playerTransform.position;
            Vector3 rotation = playerTransform.eulerAngles;
            playerInfoText.text = $"Position: ({position.x:F2}, {position.y:F2}, {position.z:F2})\n" +
                                  $"Rotation: ({rotation.x:F2}, {rotation.y:F2}, {rotation.z:F2})";
        }
    }
}

