using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    public GameObject grassPrefab;
    public int initialPoolSize = 100; // Adjust as needed

    private Dictionary<Transform, List<GameObject>> pooledObjectsByZone = new Dictionary<Transform, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    private void CreateZonePool(Transform zoneTransform)
    {
        if (!pooledObjectsByZone.ContainsKey(zoneTransform))
        {
            List<GameObject> pooledObjects = new List<GameObject>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject grassObject = Instantiate(grassPrefab);
                grassObject.SetActive(false);
                pooledObjects.Add(grassObject);
            }

            pooledObjectsByZone.Add(zoneTransform, pooledObjects);
        }
    }

    public GameObject GetPooledObject(Transform zoneTransform)
    {
        if (!pooledObjectsByZone.ContainsKey(zoneTransform))
        {
            CreateZonePool(zoneTransform);
        }

        List<GameObject> pooledObjects = pooledObjectsByZone[zoneTransform];

        foreach (GameObject grassObject in pooledObjects)
        {
            if (!grassObject.activeInHierarchy)
            {
                grassObject.SetActive(true);
                return grassObject;
            }
        }

        GameObject newGrassObject = Instantiate(grassPrefab);
        newGrassObject.SetActive(true);
        pooledObjects.Add(newGrassObject);
        return newGrassObject;
    }

    public void ReturnToPool(GameObject grassObject)
    {
        grassObject.SetActive(false);
    }
}
