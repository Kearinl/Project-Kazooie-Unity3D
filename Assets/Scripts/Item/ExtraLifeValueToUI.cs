using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ExtraLifeValueToUI : MonoBehaviour
{
    public Text extraLifeText1;
    public Text extraLifeText2;
    public float extraLifeValue = 5.0f;
    
    public static ExtraLifeValueToUI Instance { get; private set; }
    
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
        float previousValue = extraLifeValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (extraLifeValue != previousValue)
            {
                UpdateExtraLifeText();
                previousValue = extraLifeValue;
            }
        }
    }

    private void Start()
    {
        UpdateExtraLifeText();
    }

    public void UpdateExtraLifeValue(float newValue)
    {
        extraLifeValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
    }

    void UpdateExtraLifeText()
    {
        extraLifeText1.text = "" + Mathf.FloorToInt(extraLifeValue).ToString();
        extraLifeText2.text = "" + Mathf.FloorToInt(extraLifeValue).ToString();
    }
}
