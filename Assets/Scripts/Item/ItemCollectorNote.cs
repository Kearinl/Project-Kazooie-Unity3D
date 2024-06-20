using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorNote : MonoBehaviour
{
    public string itemTag = "Note";
    public delegate void OnNoteCollected(GameObject Note);
    public event OnNoteCollected NoteCollectedEvent;
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

            NoteCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newNoteValue = NoteValueToUI.Instance.NoteValue + 1;
            Debug.Log("Updating Note value from " + NoteValueToUI.Instance.NoteValue + " to " + newNoteValue);
            NoteValueToUI.Instance.UpdateNoteValue(newNoteValue);

            // Ensure the Note object is destroyed
            Debug.Log("Destroying Note object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectNoteFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectNoteFalseAfterSound()
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

