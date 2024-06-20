using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EggValueToUI : MonoBehaviour
{
    public Text EggText1;
    public Text EggText2;
    public float EggValue = 5.0f;
    
    public static EggValueToUI Instance { get; private set; }
    
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
        float previousValue = EggValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (EggValue != previousValue)
            {
                UpdateEggText();
                previousValue = EggValue;
                Debug.Log("Egg value updated: " + EggValue);
            }
        }
    }

    private void Start()
    {
        UpdateEggText();
    }

    public void UpdateEggValue(float newValue)
    {
        Debug.Log("Updating Egg value from " + EggValue + " to " + newValue);
        EggValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateEggText(); // Update the text immediately
    }

    void UpdateEggText()
    {
        EggText1.text = Mathf.FloorToInt(EggValue).ToString();
        EggText2.text = Mathf.FloorToInt(EggValue).ToString();
        Debug.Log("Egg text updated to: " + Mathf.FloorToInt(EggValue).ToString());
    }
}

