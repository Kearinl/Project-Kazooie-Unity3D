using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour
{
    public AudioClip spiralMountainMusic;
    public AudioClip bridgeToGruntildasLair1Music;
    public AudioClip underwaterSpiralMountainMusic;
    public AudioClip gruntildasLair1Music;
    public AudioClip underwaterGruntildasLair1Music;
    public AudioClip gruntildasLair1MumbosMountainMusic;
    public AudioClip gruntildasLair1TreasureTroveCoveMusic;
    public AudioClip gruntildasLair1ClankersCavernMusic;
    public AudioClip gruntildasLair1BubblegloopSwampMusic;
    public float fadeDuration = 1.0f; // Duration of the fade effect in seconds

    private AudioSource audioSource;
    private bool isInWitchHead = false;
    private bool isPlayingMusic = false;
    private AudioClip currentClip;
    private bool isInSpiralMountain = false;
    private bool isInSpiralMountain2 = false;
    private bool isFading = false; // Flag to track if music is fading
    private bool isInSpiralMountainUnderwater = false; // Flag to track if player is underwater in Spiral Mountain
    private bool isInGruntildasLair1 = false; // Flag to track if player is in Gruntilda's Lair 1
    private bool isInGruntildasLair1Underwater = false; // Flag to track if player is underwater in Gruntilda's Lair 1

    // Sub-zone flags for Gruntilda's Lair 1
    private bool isInGruntildasLair1MumbosMountain = false;
    private bool isInGruntildasLair1TreasureTroveCove = false;
    private bool isInGruntildasLair1ClankersCavern = false;
    private bool isInGruntildasLair1BubblegloopSwamp = false;

    // Adjust the volume of the music
    public float musicVolume = 1f;

    private void Start()
    {
        // Attach an AudioSource component to the same GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // Set loop to true so that the music plays continuously
        audioSource.volume = musicVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpiralMountainTrigger"))
        {
            ResetFlags();
            isInSpiralMountain = true;
            if (currentClip != spiralMountainMusic)
            {
                // Start the coroutine to fade out the current music and fade in Spiral Mountain music
                StartCoroutine(FadeMusicAndPlay(spiralMountainMusic));
            }
        }
        else if (other.CompareTag("SpiralMountainTrigger2"))
        {
            ResetFlags();
            isInSpiralMountain2 = true;
            if (currentClip != spiralMountainMusic)
            {
                // Start the coroutine to fade out the current music and fade in Spiral Mountain music
                StartCoroutine(FadeMusicAndPlay(spiralMountainMusic));
            }
        }
        else if (other.CompareTag("WitchHeadTrigger") && !isInWitchHead)
        {
            ResetFlags();
            isInWitchHead = true;
            if (currentClip != bridgeToGruntildasLair1Music)
            {
                // Start the coroutine to fade out the current music and fade in Witch Head music
                StartCoroutine(FadeMusicAndPlay(bridgeToGruntildasLair1Music));
            }
        }
        else if (other.CompareTag("UnderwaterSpiralMountainTrigger"))
        {
            ResetFlags();
            isInSpiralMountainUnderwater = true;
            if (currentClip != underwaterSpiralMountainMusic)
            {
                // Start the coroutine to fade out the current music and fade in Underwater Spiral Mountain music
                StartCoroutine(FadeMusicAndPlay(underwaterSpiralMountainMusic));
            }
        }
        else if (other.CompareTag("GruntildasLair1Trigger"))
        {
            ResetFlags();
            isInGruntildasLair1 = true;
            if (currentClip != gruntildasLair1Music)
            {
                // Start the coroutine to fade out the current music and fade in Gruntilda's Lair 1 music
                StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
            }
        }
        else if (other.CompareTag("UnderwaterGruntildasLair1Trigger"))
        {
            ResetFlags();
            isInGruntildasLair1Underwater = true;
            if (currentClip != underwaterGruntildasLair1Music)
            {
                // Start the coroutine to fade out the current music and fade in Underwater Gruntilda's Lair 1 music
                StartCoroutine(FadeMusicAndPlay(underwaterGruntildasLair1Music));
            }
        }
        else if (other.CompareTag("GruntildasLair1MumbosMountainTrigger"))
        {
            ResetFlags();
            isInGruntildasLair1MumbosMountain = true;
            if (currentClip != gruntildasLair1MumbosMountainMusic)
            {
                // Start the coroutine to fade out the current music and fade in Mumbo's Mountain music
                StartCoroutine(FadeMusicAndPlay(gruntildasLair1MumbosMountainMusic));
            }
        }
        else if (other.CompareTag("GruntildasLair1TreasureTroveCoveTrigger"))
        {
            ResetFlags();
            isInGruntildasLair1TreasureTroveCove = true;
            if (currentClip != gruntildasLair1TreasureTroveCoveMusic)
            {
                // Start the coroutine to fade out the current music and fade in Treasure Trove Cove music
                StartCoroutine(FadeMusicAndPlay(gruntildasLair1TreasureTroveCoveMusic));
            }
        }
        else if (other.CompareTag("GruntildasLair1ClankersCavernTrigger"))
        {
            ResetFlags();
            isInGruntildasLair1ClankersCavern = true;
            if (currentClip != gruntildasLair1ClankersCavernMusic)
            {
                // Start the coroutine to fade out the current music and fade in Clanker's Cavern music
                StartCoroutine(FadeMusicAndPlay(gruntildasLair1ClankersCavernMusic));
            }
        }
        else if (other.CompareTag("GruntildasLair1BubblegloopSwampTrigger"))
        {
            ResetFlags();
            isInGruntildasLair1BubblegloopSwamp = true;
            if (currentClip != gruntildasLair1BubblegloopSwampMusic)
            {
                // Start the coroutine to fade out the current music and fade in Bubblegloop Swamp music
                StartCoroutine(FadeMusicAndPlay(gruntildasLair1BubblegloopSwampMusic));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WitchHeadTrigger"))
        {
            isInWitchHead = false;
            // Start the coroutine to fade out the current music
            StartCoroutine(FadeOutMusic());
        }
        else if (other.CompareTag("UnderwaterSpiralMountainTrigger"))
        {
            isInSpiralMountainUnderwater = false;
            // Check if still in Spiral Mountain
            if (isInSpiralMountain)
            {
                if (currentClip != spiralMountainMusic)
                {
                    StartCoroutine(FadeMusicAndPlay(spiralMountainMusic));
                }
            }
            else
            {
                // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
        else if (other.CompareTag("GruntildasLair1Trigger"))
        {
            isInGruntildasLair1 = false;
            // Start the coroutine to fade out the current music
            StartCoroutine(FadeOutMusic());
        }
        else if (other.CompareTag("UnderwaterGruntildasLair1Trigger"))
        {
            isInGruntildasLair1Underwater = false;
            // Check if still in Gruntilda's Lair 1
            if (isInGruntildasLair1)
            {
                if (currentClip != gruntildasLair1Music)
                {
                    StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
                }
            }
            else
            {
                // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
        else if (other.CompareTag("GruntildasLair1MumbosMountainTrigger"))
        {
            isInGruntildasLair1MumbosMountain = false;
            // Check if still in Gruntilda's Lair 1
            if (isInGruntildasLair1)
            {
                if (currentClip != gruntildasLair1Music)
                {
                    StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
                }
            }
            else
            {
              // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
        else if (other.CompareTag("GruntildasLair1TreasureTroveCoveTrigger"))
        {
            isInGruntildasLair1TreasureTroveCove = false;
            // Check if still in Gruntilda's Lair 1
            if (isInGruntildasLair1)
            {
                if (currentClip != gruntildasLair1Music)
                {
                    StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
                }
            }
            else
            {
                // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
        else if (other.CompareTag("GruntildasLair1ClankersCavernTrigger"))
        {
            isInGruntildasLair1ClankersCavern = false;
            // Check if still in Gruntilda's Lair 1
            if (isInGruntildasLair1)
            {
                if (currentClip != gruntildasLair1Music)
                {
                    StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
                }
            }
            else
            {
                // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
        else if (other.CompareTag("GruntildasLair1BubblegloopSwampTrigger"))
        {
            isInGruntildasLair1BubblegloopSwamp = false;
            // Check if still in Gruntilda's Lair 1
            if (isInGruntildasLair1)
            {
                if (currentClip != gruntildasLair1Music)
                {
                    StartCoroutine(FadeMusicAndPlay(gruntildasLair1Music));
                }
            }
            else
            {
                // Start the coroutine to fade out the current music
                StartCoroutine(FadeOutMusic());
            }
        }
    }

    private void ResetFlags()
    {
        isInSpiralMountain = false;
        isInSpiralMountain2 = false;
        isInWitchHead = false;
        isInSpiralMountainUnderwater = false;
        isInGruntildasLair1 = false;
        isInGruntildasLair1Underwater = false;
        isInGruntildasLair1MumbosMountain = false;
        isInGruntildasLair1TreasureTroveCove = false;
        isInGruntildasLair1ClankersCavern = false;
        isInGruntildasLair1BubblegloopSwamp = false;
    }

    private IEnumerator FadeMusicAndPlay(AudioClip newClip)
    {
        if (isFading) yield break; // Prevent overlapping fades
        isFading = true;
        
        if (audioSource.isPlaying)
        {
            // Fade out current music
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume;
        }

        // Play new music
        currentClip = newClip;
        audioSource.clip = newClip;
        audioSource.Play();
        
        // Fade in new music
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, musicVolume, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = musicVolume;
        isFading = false;
    }

    private IEnumerator FadeOutMusic()
    {
        if (isFading) yield break; // Prevent overlapping fades
        isFading = true;

        if (audioSource.isPlaying)
        {
            // Fade out current music
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            audioSource.Stop();
            audioSource.volume = startVolume;
        }

        isFading = false;
    }
}
