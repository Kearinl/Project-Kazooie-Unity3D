using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public Transform targetZoneTransform; // Assign the target zone's transform in the Inspector

private void Start()
{
    if (targetZoneTransform == null)
    {
        Debug.LogError("Target zone transform not assigned!");
        return;
    }

    GameObject grassObject = ObjectPoolManager.Instance.GetPooledObject(targetZoneTransform);

    if (grassObject != null)
    {
        grassObject.transform.position = targetZoneTransform.position; // Set the grass position
        Debug.Log("Grass spawned successfully!");
    }
    else
    {
        Debug.LogWarning("No available grass objects in the pool!");
    }
}

}
