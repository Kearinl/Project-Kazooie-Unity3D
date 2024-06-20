using UnityEngine;

public class ItemUsage : MonoBehaviour
{
    // Reference to the Banjo character with the ItemCollector script attached
    public GameObject banjo;

    private void Start()
    {
        // Subscribe to the ItemCollectedEvent
        ItemCollector itemCollector = banjo.GetComponent<ItemCollector>();
        if (itemCollector != null)
        {
            itemCollector.ItemCollectedEvent += OnItemCollected;
        }
    }

    // This method will be called whenever an item is collected
    private void OnItemCollected(GameObject item)
    {
        // Do something with the collected item
        Debug.Log("Banjo collected: " + item.name);
    }
}
