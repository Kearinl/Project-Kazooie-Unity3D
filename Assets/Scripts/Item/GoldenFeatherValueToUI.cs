using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoldenFeatherValueToUI : MonoBehaviour
{
    public Text GoldenFeatherText1;
    public Text GoldenFeatherText2;
    public float GoldenFeatherValue = 5.0f;
    
    public static GoldenFeatherValueToUI Instance { get; private set; }
    
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
        float previousValue = GoldenFeatherValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (GoldenFeatherValue != previousValue)
            {
                UpdateGoldenFeatherText();
                previousValue = GoldenFeatherValue;
                Debug.Log("GoldenFeather value updated: " + GoldenFeatherValue);
            }
        }
    }

    private void Start()
    {
        UpdateGoldenFeatherText();
    }

    public void UpdateGoldenFeatherValue(float newValue)
    {
        Debug.Log("Updating GoldenFeather value from " + GoldenFeatherValue + " to " + newValue);
        GoldenFeatherValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateGoldenFeatherText(); // Update the text immediately
    }

    void UpdateGoldenFeatherText()
    {
        GoldenFeatherText1.text = Mathf.FloorToInt(GoldenFeatherValue).ToString();
        GoldenFeatherText2.text = Mathf.FloorToInt(GoldenFeatherValue).ToString();
        Debug.Log("GoldenFeather text updated to: " + Mathf.FloorToInt(GoldenFeatherValue).ToString());
    }
}

