using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class RedFeatherValueToUI : MonoBehaviour
{
    public Text RedFeatherText1;
    public Text RedFeatherText2;
    public float RedFeatherValue = 5.0f;
    
    public static RedFeatherValueToUI Instance { get; private set; }
    
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
        float previousValue = RedFeatherValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (RedFeatherValue != previousValue)
            {
                UpdateRedFeatherText();
                previousValue = RedFeatherValue;
                Debug.Log("RedFeather value updated: " + RedFeatherValue);
            }
        }
    }

    private void Start()
    {
        UpdateRedFeatherText();
    }

    public void UpdateRedFeatherValue(float newValue)
    {
        Debug.Log("Updating RedFeather value from " + RedFeatherValue + " to " + newValue);
        RedFeatherValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateRedFeatherText(); // Update the text immediately
    }

    void UpdateRedFeatherText()
    {
        RedFeatherText1.text = Mathf.FloorToInt(RedFeatherValue).ToString();
        RedFeatherText2.text = Mathf.FloorToInt(RedFeatherValue).ToString();
        Debug.Log("RedFeather text updated to: " + Mathf.FloorToInt(RedFeatherValue).ToString());
    }
}

