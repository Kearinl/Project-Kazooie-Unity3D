using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorHoneyComb : MonoBehaviour
{
    public string itemTag = "HoneyComb";
    public delegate void OnHoneyCombCollected(GameObject HoneyComb);
    public event OnHoneyCombCollected HoneyCombCollectedEvent;
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

            HoneyCombCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newHoneyCombValue = HoneyCombValueToUI.Instance.HoneyCombValue + 1;
            Debug.Log("Updating HoneyComb value from " + HoneyCombValueToUI.Instance.HoneyCombValue + " to " + newHoneyCombValue);
            HoneyCombValueToUI.Instance.UpdateHoneyCombValue(newHoneyCombValue);

            // Ensure the HoneyComb object is destroyed
            Debug.Log("Destroying HoneyComb object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectHoneyCombFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectHoneyCombFalseAfterSound()
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

