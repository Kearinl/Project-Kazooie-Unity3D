using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public int score;
    public string lastScene;
}

public class SaveLoadManager : MonoBehaviour
{
    public static void SavePlayerData(PlayerData data)
    {
        data.lastScene = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerData", json);
    }

    public static PlayerData LoadPlayerData()
    {
        string json = PlayerPrefs.GetString("PlayerData", "");
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        return playerData;
    }
}
