using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MumboTokenValueToUI : MonoBehaviour
{
    public Text MumboTokenText1;
    public Text MumboTokenText2;
    public float MumboTokenValue = 5.0f;
    
    public static MumboTokenValueToUI Instance { get; private set; }
    
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
        float previousValue = MumboTokenValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (MumboTokenValue != previousValue)
            {
                UpdateMumboTokenText();
                previousValue = MumboTokenValue;
                Debug.Log("MumboToken value updated: " + MumboTokenValue);
            }
        }
    }

    private void Start()
    {
        UpdateMumboTokenText();
    }

    public void UpdateMumboTokenValue(float newValue)
    {
        Debug.Log("Updating MumboToken value from " + MumboTokenValue + " to " + newValue);
        MumboTokenValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateMumboTokenText(); // Update the text immediately
    }

    void UpdateMumboTokenText()
    {
        MumboTokenText1.text = Mathf.FloorToInt(MumboTokenValue).ToString();
        MumboTokenText2.text = Mathf.FloorToInt(MumboTokenValue).ToString();
        Debug.Log("MumboToken text updated to: " + Mathf.FloorToInt(MumboTokenValue).ToString());
    }
}

