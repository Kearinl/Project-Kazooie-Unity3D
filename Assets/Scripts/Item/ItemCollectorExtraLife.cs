using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemCollectorExtraLife : MonoBehaviour
{
    // The tag of the items you want Banjo to collect
    public string itemTag = "ExtraLife";


    // Event to notify when an item is collected
    public delegate void OnItemCollected(GameObject item);
    public event OnItemCollected ItemCollectedEvent;

    // Reference to the audio clip for playing the collection sound
    public AudioClip collectionSoundClip;

    // Adjust the volume of the collection sound
    public float collectionVolume = 1f;

    // Adjust the range of the collection sound (how far it can be heard)
    public float collectionRange = 10f;
    
    public GameObject activationGameObject; // Reference to the GameObject you want to activate and then deactivate
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the specified tag
        if (other.CompareTag(itemTag))
        {
            // Notify that an item has been collected
            ItemCollectedEvent?.Invoke(other.gameObject);

            // Play the collection sound if available
            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }
            
        // Update the ExtraLife value by adding 1
            ExtraLifeValueToUI.Instance.extraLifeValue += 1;
            
            // Destroy the collected item
            Destroy(other.gameObject);
            
            // Activate the GameObject reference
            activationGameObject.SetActive(true);

            // Start the coroutine to disable the GameObject after a delay
            StartCoroutine(DeactivateGameObjectAfterDelay());
            
        }
    }
    
    private System.Collections.IEnumerator DeactivateGameObjectAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Deactivate the GameObject reference
        activationGameObject.SetActive(false);
    }

    private void PlayCollectionSound()
    {
        // Create a temporary GameObject to act as the audio source
        GameObject audioSourceGO = new GameObject("CollectionAudioSource");
        audioSourceGO.transform.position = transform.position;

        // Add an AudioSource component to the temporary GameObject
        AudioSource audioSource = audioSourceGO.AddComponent<AudioSource>();

        // Set audio source properties
        audioSource.clip = collectionSoundClip;
        audioSource.volume = collectionVolume;
        audioSource.maxDistance = collectionRange;

        // Play the collection sound
        audioSource.Play();

        // Destroy the temporary GameObject after the clip finishes playing
        Destroy(audioSourceGO, collectionSoundClip.length);
    }
}
