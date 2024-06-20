using UnityEngine;
using System.Collections;

public class SpirialMountainTriggerController : MonoBehaviour
{
    public GameObject objectToEnable1; // New object to enable
    public GameObject objectToDisable1; // New object to disable
    public GameObject objectToEnable2; // Another new object to enable
    public GameObject objectToDisable2; // Another new object to disable
    public GameObject uiObjectToEnable;
    public GameObject uiObjectToEnable2;
    public GameObject skyObjectToDisable;
    public GameObject skyObjectToEnable;
    public Collider otherTriggerCollider; // Reference to the other trigger's collider

    public float cooldownTime = 10f; // Cooldown time in seconds
    private float lastTriggerTime;

    private void Start()
    {
        lastTriggerTime = -cooldownTime; // Initialize to allow immediate use
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastTriggerTime + cooldownTime)
        {
            lastTriggerTime = Time.time; // Update the last trigger time
            PerformActions(); // Perform the sequence of actions
            StartCoroutine(DisableOtherTriggerColliderTemporarily()); // Disable the other trigger collider
        }
    }

    private void PerformActions()
    {
        // Disable objects if specified
        if (objectToDisable1 != null)
        {
            objectToDisable1.SetActive(false);
            Collider disableCollider = objectToDisable1.GetComponent<Collider>();
            if (disableCollider != null)
            {
                disableCollider.enabled = false;
            }
        }

        if (objectToDisable2 != null)
        {
            objectToDisable2.SetActive(false);
            Collider disableCollider = objectToDisable2.GetComponent<Collider>();
            if (disableCollider != null)
            {
                disableCollider.enabled = false;
            }
        }

        // Enable objects if specified
        if (objectToEnable1 != null)
        {
            objectToEnable1.SetActive(true);
            Collider enableCollider = objectToEnable1.GetComponent<Collider>();
            if (enableCollider != null)
            {
                enableCollider.enabled = false;
                StartCoroutine(ReenableColliderAfterCooldown(objectToEnable1));
            }
        }

        if (objectToEnable2 != null)
        {
            objectToEnable2.SetActive(true);
            Collider enableCollider = objectToEnable2.GetComponent<Collider>();
            if (enableCollider != null)
            {
                enableCollider.enabled = false;
                StartCoroutine(ReenableColliderAfterCooldown(objectToEnable2));
            }
        }

        // Enable UI objects with delays
        if (uiObjectToEnable != null)
        {
            uiObjectToEnable.SetActive(true);
            StartCoroutine(DisableUIObjectWithDelay(uiObjectToEnable));
        }

        if (uiObjectToEnable2 != null)
        {
            uiObjectToEnable2.SetActive(true);
            StartCoroutine(DisableUIObjectWithDelay(uiObjectToEnable2));
        }

        // Enable or disable sky objects
        if (skyObjectToDisable != null)
        {
            skyObjectToDisable.SetActive(false);
        }

        if (skyObjectToEnable != null)
        {
            skyObjectToEnable.SetActive(true);
        }
    }

    private IEnumerator DisableUIObjectWithDelay(GameObject uiObject)
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    private IEnumerator ReenableColliderAfterCooldown(GameObject obj)
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for cooldown time
        Collider objectCollider = obj.GetComponent<Collider>();
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
    }

    private IEnumerator DisableOtherTriggerColliderTemporarily()
    {
        if (otherTriggerCollider != null)
        {
            otherTriggerCollider.enabled = false; // Disable the other trigger collider
            yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
            otherTriggerCollider.enabled = true; // Re-enable the other trigger collider
        }
    }
}

