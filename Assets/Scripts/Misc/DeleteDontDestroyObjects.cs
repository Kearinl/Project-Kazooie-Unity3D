using UnityEngine;

public class DeleteDontDestroyObjects : MonoBehaviour
{
    private void Awake()
    {
        // Show the cursor again
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Find all objects with the "Don'tDestroyOnLoad" flag set
        GameObject[] dontDestroyObjects = GameObject.FindGameObjectsWithTag("DontDestroy");

        // Delete each of these objects
        foreach (GameObject obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }
}
