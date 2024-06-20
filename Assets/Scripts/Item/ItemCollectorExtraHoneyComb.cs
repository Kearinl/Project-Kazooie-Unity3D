using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorExtraHoneyComb : MonoBehaviour
{
    public string itemTag = "ExtraHoneyComb";
    public delegate void OnExtraHoneyCombCollected(GameObject ExtraHoneyComb);
    public event OnExtraHoneyCombCollected ExtraHoneyCombCollectedEvent;
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

            ExtraHoneyCombCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newExtraHoneyCombValue = ExtraHoneyCombValueToUI.Instance.ExtraHoneyCombValue + 1;
            Debug.Log("Updating ExtraHoneyComb value from " + ExtraHoneyCombValueToUI.Instance.ExtraHoneyCombValue + " to " + newExtraHoneyCombValue);
            ExtraHoneyCombValueToUI.Instance.UpdateExtraHoneyCombValue(newExtraHoneyCombValue);

            // Ensure the ExtraHoneyComb object is destroyed
            Debug.Log("Destroying ExtraHoneyComb object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectExtraHoneyCombFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectExtraHoneyCombFalseAfterSound()
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

