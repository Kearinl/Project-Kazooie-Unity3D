using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class NoteValueToUI : MonoBehaviour
{
    public Text NoteText1;
    public Text NoteText2;
    public float NoteValue = 5.0f;
    
    public static NoteValueToUI Instance { get; private set; }
    
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
        float previousValue = NoteValue;

        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (NoteValue != previousValue)
            {
                UpdateNoteText();
                previousValue = NoteValue;
                Debug.Log("Note value updated: " + NoteValue);
            }
        }
    }

    private void Start()
    {
        UpdateNoteText();
    }

    public void UpdateNoteValue(float newValue)
    {
        Debug.Log("Updating Note value from " + NoteValue + " to " + newValue);
        NoteValue = Mathf.Max(0f, newValue); // Ensure the value doesn't go below 0
        UpdateNoteText(); // Update the text immediately
    }

    void UpdateNoteText()
    {
        NoteText1.text = Mathf.FloorToInt(NoteValue).ToString();
        NoteText2.text = Mathf.FloorToInt(NoteValue).ToString();
        Debug.Log("Note text updated to: " + Mathf.FloorToInt(NoteValue).ToString());
    }
}

