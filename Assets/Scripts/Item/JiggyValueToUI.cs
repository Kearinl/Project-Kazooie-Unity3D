using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class JiggyValueToUI : MonoBehaviour
{
    public Text jiggyText1;
    public Text jiggyText2;
    public float jiggyValue = 5.0f;
    
    public static JiggyValueToUI Instance { get; private set; }
    
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
        float previousValue = jiggyValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (jiggyValue != previousValue)
            {
                UpdateJiggyText();
                previousValue = jiggyValue;
                Debug.Log("Jiggy value updated: " + jiggyValue);
            }
        }
    }

    private void Start()
    {
        UpdateJiggyText();
    }

    public void UpdateJiggyValue(float newValue)
    {
        Debug.Log("Updating jiggy value from " + jiggyValue + " to " + newValue);
        jiggyValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateJiggyText(); // Update the text immediately
    }

    void UpdateJiggyText()
    {
        jiggyText1.text = Mathf.FloorToInt(jiggyValue).ToString();
        jiggyText2.text = Mathf.FloorToInt(jiggyValue).ToString();
        Debug.Log("Jiggy text updated to: " + Mathf.FloorToInt(jiggyValue).ToString());
    }
}

