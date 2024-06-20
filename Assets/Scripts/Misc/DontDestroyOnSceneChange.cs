using UnityEngine;

public class DontDestroyOnSceneChange : MonoBehaviour
{
    private static DontDestroyOnSceneChange _instance;

    private void Awake()
    {
        // Check if an instance already exists
        if (_instance == null)
        {
            // If not, set this instance as the singleton instance
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate instance
            Destroy(gameObject);
        }
    }
}
