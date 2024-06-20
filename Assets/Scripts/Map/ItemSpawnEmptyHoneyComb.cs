using UnityEngine;

public class ItemSpawnEmptyHoneyComb : MonoBehaviour
{
    public GameObject itemToEnable; // Assign the item GameObject to enable

    private void OnDisable()
    {
        EnableItem();
    }

    private void EnableItem()
    {
        if (itemToEnable != null)
        {
            itemToEnable.SetActive(true);
        }
    }
}
