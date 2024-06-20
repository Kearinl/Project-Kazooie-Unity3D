using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExtraHoneyCombValueToUI : MonoBehaviour
{
    public Text ExtraHoneyCombText1;
    public Text ExtraHoneyCombText2;
    public float ExtraHoneyCombValue = 5.0f;
    
    public static ExtraHoneyCombValueToUI Instance { get; private set; }
    
    private float checkInterval = 0.3f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Check if the current scene is "GameOver" or "MainMenu"
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "GameOver" || currentScene.name == "MainMenu")
        {
            Destroy(gameObject);
        }

        StartCheckingValueChange();
    }

    private void StartCheckingValueChange()
    {
        StartCoroutine(CheckValueChange());
    }
    
    private IEnumerator CheckValueChange()
    {
        float previousValue = ExtraHoneyCombValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (ExtraHoneyCombValue != previousValue)
            {
                UpdateExtraHoneyCombText();
                previousValue = ExtraHoneyCombValue;
                Debug.Log("ExtraHoneyComb value updated: " + ExtraHoneyCombValue);
            }
        }
    }

    private void Start()
    {
        UpdateExtraHoneyCombText();
    }

    public void UpdateExtraHoneyCombValue(float newValue)
    {
        Debug.Log("Updating ExtraHoneyComb value from " + ExtraHoneyCombValue + " to " + newValue);
        ExtraHoneyCombValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateExtraHoneyCombText(); // Update the text immediately
    }

    void UpdateExtraHoneyCombText()
    {
        ExtraHoneyCombText1.text = Mathf.FloorToInt(ExtraHoneyCombValue).ToString();
        ExtraHoneyCombText2.text = Mathf.FloorToInt(ExtraHoneyCombValue).ToString();
        Debug.Log("ExtraHoneyComb text updated to: " + Mathf.FloorToInt(ExtraHoneyCombValue).ToString());
    }
}

