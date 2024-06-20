using UnityEngine;

public class EnableDisableUI : MonoBehaviour
{
    public GameObject uiObject;

    private void Start()
    {
        // Enable the UI object when the scene is loaded
        uiObject.SetActive(true);

        // Call the DisableUIObject method after 5 seconds
        Invoke("DisableUIObject", 3f);
    }

    private void DisableUIObject()
    {
        // Disable the UI object after 5 seconds
        uiObject.SetActive(false);
    }
}
