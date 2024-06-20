using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class ItemCollectorJiggy : MonoBehaviour
{
    public string itemTag = "Jiggy";
    private Animator _animator;
    private PlayerInput _playerInput; // Reference to the PlayerInput component
    private CharacterController _characterController;
    public delegate void OnJiggyCollected(GameObject jiggy);
    public event OnJiggyCollected JiggyCollectedEvent;
    public AudioClip collectionSoundClip;
    public float collectionVolume = 1f;
    public float collectionRange = 10f;
    public GameObject uiGameObject; // Reference to the UI GameObject you want to activate and then deactivate

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found on the same GameObject as ItemCollectorJiggy script.");
        }

        _characterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(itemTag))
        {
            Debug.Log("Collected item with tag: " + itemTag);

            JiggyCollectedEvent?.Invoke(other.gameObject);

            if (collectionSoundClip != null)
            {
                PlayCollectionSound();
            }

            if (_animator != null)
            {
                _animator.SetBool("collectJiggy", true);
            }

            float newJiggyValue = JiggyValueToUI.Instance.jiggyValue + 1;
            Debug.Log("Updating jiggy value from " + JiggyValueToUI.Instance.jiggyValue + " to " + newJiggyValue);
            JiggyValueToUI.Instance.UpdateJiggyValue(newJiggyValue);

            // Ensure the jiggy object is destroyed
            Debug.Log("Destroying jiggy object: " + other.gameObject.name);
            Destroy(other.gameObject);

            if (uiGameObject != null)
            {
                uiGameObject.SetActive(true);
            }

            if (_characterController != null)
            {
                _characterController.enabled = false;
            }

            StartCoroutine(SetCollectJiggyFalseAfterSound());
            StartCoroutine(DeactivateGameObjectAfterDelay());
        }
    }

    private IEnumerator SetCollectJiggyFalseAfterSound()
    {
        if (collectionSoundClip != null)
        {
            yield return new WaitForSeconds(collectionSoundClip.length);
        }
        else
        {
            yield return null;
        }

        if (_animator != null)
        {
            _animator.SetBool("collectJiggy", false);
        }

        if (_characterController != null)
        {
            _characterController.enabled = true;
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

