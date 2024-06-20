using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorGoldenFeather : MonoBehaviour
{
    public string itemTag = "GoldenFeather";
    public delegate void OnGoldenFeatherCollected(GameObject GoldenFeather);
    public event OnGoldenFeatherCollected GoldenFeatherCollectedEvent;
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

            GoldenFeatherCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newGoldenFeatherValue = GoldenFeatherValueToUI.Instance.GoldenFeatherValue + 1;
            Debug.Log("Updating GoldenFeather value from " + GoldenFeatherValueToUI.Instance.GoldenFeatherValue + " to " + newGoldenFeatherValue);
            GoldenFeatherValueToUI.Instance.UpdateGoldenFeatherValue(newGoldenFeatherValue);

            // Ensure the GoldenFeather object is destroyed
            Debug.Log("Destroying GoldenFeather object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectGoldenFeatherFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectGoldenFeatherFalseAfterSound()
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

