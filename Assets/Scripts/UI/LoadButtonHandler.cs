using UnityEngine;

public class LoadButtonHandler : MonoBehaviour
{
    public void LoadPlayerData()
    {
        PlayerData loadedData = SaveLoadManager.LoadPlayerData();
        // You can handle the loaded data here
    }
}
