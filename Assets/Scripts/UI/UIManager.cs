using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public Text noteCountText;     // Reference to the Note count Text
    public Text extraLifeCountText; // Reference to the ExtraLife count Text
    // Add references to other GUI elements as needed

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static UIManager Instance
    {
        get { return instance; }
    }
    
       public Text GetItemCountText(string collectableTag)
    {
        switch (collectableTag)
        {
            case "Note":
                return noteCountText;
            case "ExtraLife":
                return extraLifeCountText;
            // Add cases for other collectable tags
            default:
                return null;
        }
    }
}
