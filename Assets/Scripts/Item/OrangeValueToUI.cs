using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OrangeValueToUI : MonoBehaviour
{
    public Text OrangeText1;
    public Text OrangeText2;
    public float OrangeValue = 5.0f;
    
    public static OrangeValueToUI Instance { get; private set; }
    
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
        float previousValue = OrangeValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (OrangeValue != previousValue)
            {
                UpdateOrangeText();
                previousValue = OrangeValue;
                Debug.Log("Orange value updated: " + OrangeValue);
            }
        }
    }

    private void Start()
    {
        UpdateOrangeText();
    }

    public void UpdateOrangeValue(float newValue)
    {
        Debug.Log("Updating Orange value from " + OrangeValue + " to " + newValue);
        OrangeValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateOrangeText(); // Update the text immediately
    }

    void UpdateOrangeText()
    {
        OrangeText1.text = Mathf.FloorToInt(OrangeValue).ToString();
        OrangeText2.text = Mathf.FloorToInt(OrangeValue).ToString();
        Debug.Log("Orange text updated to: " + Mathf.FloorToInt(OrangeValue).ToString());
    }
}

