using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class HoneyCombValueToUI : MonoBehaviour
{
    public Text HoneyCombText1;
    public Text HoneyCombText2;
    public float HoneyCombValue = 5.0f;
    
    public static HoneyCombValueToUI Instance { get; private set; }
    
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
        float previousValue = HoneyCombValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (HoneyCombValue != previousValue)
            {
                UpdateHoneyCombText();
                previousValue = HoneyCombValue;
                Debug.Log("HoneyComb value updated: " + HoneyCombValue);
            }
        }
    }

    private void Start()
    {
        UpdateHoneyCombText();
    }

    public void UpdateHoneyCombValue(float newValue)
    {
        Debug.Log("Updating HoneyComb value from " + HoneyCombValue + " to " + newValue);
        HoneyCombValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateHoneyCombText(); // Update the text immediately
    }

    void UpdateHoneyCombText()
    {
        HoneyCombText1.text = Mathf.FloorToInt(HoneyCombValue).ToString();
        HoneyCombText2.text = Mathf.FloorToInt(HoneyCombValue).ToString();
        Debug.Log("HoneyComb text updated to: " + Mathf.FloorToInt(HoneyCombValue).ToString());
    }
}

