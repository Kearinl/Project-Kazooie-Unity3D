using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorRedFeather : MonoBehaviour
{
    public string itemTag = "RedFeather";
    public delegate void OnRedFeatherCollected(GameObject RedFeather);
    public event OnRedFeatherCollected RedFeatherCollectedEvent;
    public AudioClip collectionSoundClip;
    public float collectionVolume = 1f;
    public float collectionRange = 10f;
    public GameObject uiGameObject; // Reference to the UI GameObject you want to activate and then deactivate

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemTag))
        {
            Debug.Log("Collected item with tag: " + itemTag);

            RedFeatherCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newRedFeatherValue = RedFeatherValueToUI.Instance.RedFeatherValue + 1;
            Debug.Log("Updating RedFeather value from " + RedFeatherValueToUI.Instance.RedFeatherValue + " to " + newRedFeatherValue);
            RedFeatherValueToUI.Instance.UpdateRedFeatherValue(newRedFeatherValue);

            // Ensure the RedFeather object is destroyed
            Debug.Log("Destroying RedFeather object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectRedFeatherFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectRedFeatherFalseAfterSound()
    {
        if (collectionSoundClip != null)
        {
            yield return new WaitForSeconds(collectionSoundClip.length);
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator DeactivateGameObjectAfterDelay()
    {
        yield return new WaitForSeconds(3);

        if (uiGameObject != null)
        {
            uiGameObject.SetActive(false);
        }
    }

    private void PlayCollectionSound()
    {
        GameObject audioSourceGO = new GameObject("CollectionAudioSource");
        audioSourceGO.transform.position = transform.position;

        AudioSource audioSource = audioSourceGO.AddComponent<AudioSource>();
        audioSource.clip = collectionSoundClip;
        audioSource.volume = collectionVolume;
        audioSource.maxDistance = collectionRange;

        audioSource.Play();
        Destroy(audioSourceGO, collectionSoundClip.length);
    }
}

