using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorEgg : MonoBehaviour
{
    public string itemTag = "Egg";
    public delegate void OnEggCollected(GameObject Egg);
    public event OnEggCollected EggCollectedEvent;
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

            EggCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newEggValue = EggValueToUI.Instance.EggValue + 1;
            Debug.Log("Updating Egg value from " + EggValueToUI.Instance.EggValue + " to " + newEggValue);
            EggValueToUI.Instance.UpdateEggValue(newEggValue);

            // Ensure the Egg object is destroyed
            Debug.Log("Destroying Egg object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectEggFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectEggFalseAfterSound()
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

