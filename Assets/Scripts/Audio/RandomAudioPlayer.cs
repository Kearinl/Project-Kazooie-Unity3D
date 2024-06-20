using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    public AudioClip audioClip3;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.loop = false; // Ensure the audio source does not loop the clip itself
        PlayRandomClip(); // Call the method to play a random clip at the start
    }

    void Update()
    {
        if (!audioSource.isPlaying && audioSource.time == 0)
        {
            PlayRandomClip(); // Play a new random clip when the current one finishes
        }
    }

    public void PlayRandomClip()
    {
        int randomIndex = Random.Range(0, 3);
        AudioClip clipToPlay = null;

        switch (randomIndex)
        {
            case 0:
                clipToPlay = audioClip1;
                break;
            case 1:
                clipToPlay = audioClip2;
                break;
            case 2:
                clipToPlay = audioClip3;
                break;
        }

        audioSource.clip = clipToPlay;
        audioSource.Play();
    }
}

