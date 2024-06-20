using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    // The tag of the items you want Banjo to collect
    public string itemTag = "Collectible";

    // Event to notify when an item is collected
    public delegate void OnItemCollected(GameObject item);
    public event OnItemCollected ItemCollectedEvent;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(itemTag))
        {
            // Notify that an item has been collected
            ItemCollectedEvent?.Invoke(other.gameObject);

            // Destroy the collected item
            Destroy(other.gameObject);
        }
    }
}
