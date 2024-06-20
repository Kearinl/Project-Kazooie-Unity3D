using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    public AudioClip audioClip;
    public GameObject triggerObject;
    private bool isInsideTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == triggerObject)
        {
            isInsideTrigger = true;
            Debug.Log("Entered trigger zone: " + triggerObject.name);
            PlayAudioClip();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == triggerObject)
        {
            isInsideTrigger = false;
            Debug.Log("Exited trigger zone: " + triggerObject.name);
            StopAudioClip();
        }
    }

    private void PlayAudioClip()
    {
        if (audioClip != null)
        {
            // Play the audio clip at the position of the trigger GameObject
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
        }
    }

    private void StopAudioClip()
    {
        // Nothing to stop since we are using PlayClipAtPoint
        // PlayClipAtPoint doesn't require stopping explicitly
    }
}
