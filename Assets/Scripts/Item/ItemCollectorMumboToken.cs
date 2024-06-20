using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemCollectorMumboToken : MonoBehaviour
{
    public string itemTag = "MumboToken";
    public delegate void OnMumboTokenCollected(GameObject MumboToken);
    public event OnMumboTokenCollected MumboTokenCollectedEvent;
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

            MumboTokenCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            float newMumboTokenValue = MumboTokenValueToUI.Instance.MumboTokenValue + 1;
            Debug.Log("Updating MumboToken value from " + MumboTokenValueToUI.Instance.MumboTokenValue + " to " + newMumboTokenValue);
            MumboTokenValueToUI.Instance.UpdateMumboTokenValue(newMumboTokenValue);

            // Ensure the MumboToken object is destroyed
            Debug.Log("Destroying MumboToken object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }


            StartCoroutine(SetCollectMumboTokenFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectMumboTokenFalseAfterSound()
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

