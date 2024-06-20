using UnityEngine;
using System.Collections;

public class DisableGameObject : MonoBehaviour
{
    public GameObject objectToDisable;

    private void Start()
    {
        StartCoroutine(DisableObjectWithDelay());
    }

    private IEnumerator DisableObjectWithDelay()
    {
        yield return new WaitForSeconds(2f); // Wait for 4 seconds
        
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
    }
}
