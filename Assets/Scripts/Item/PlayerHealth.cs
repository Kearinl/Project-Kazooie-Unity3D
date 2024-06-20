using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;

    public float hitForce = 20f;
    public float damageImmunityTime = 1f;
    private bool isImmuneToDamage = false;
    
    private bool canDie = true; // Flag to control if Die function can be called
    private float dieCooldown = 10f; // Cooldown duration in seconds

    public AudioClip hitSound; // Reference to the hit sound AudioClip.

    private CharacterController characterController;
    
    public GameObject activationGameObject; // Reference to the GameObject you want to activate and then deactivate

    private void Start()
    {
        currentHealth = maxHealth;
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isImmuneToDamage)
        {
            damageImmunityTime -= Time.deltaTime;
            if (damageImmunityTime <= 0f)
            {
                isImmuneToDamage = false;
            }
        }
        
        // Update the cooldown timer
    if (!canDie)
    {
        dieCooldown -= Time.deltaTime;
        if (dieCooldown <= 0f)
        {
            canDie = true;
            dieCooldown = 10f; // Reset the cooldown
        }
    }
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isImmuneToDamage)
        {
            currentHealth -= damageAmount;
            Debug.Log("Player took " + damageAmount + " damage!");

            // Calculate knockback direction based on player's movement
            Vector3 knockbackDirection = transform.forward * -1f;
            knockbackDirection.y = 0;

            // Apply knockback effect
            Vector3 knockbackVector = knockbackDirection * hitForce;
            characterController.Move(knockbackVector * Time.deltaTime);

            // Play hit sound
            if (hitSound)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                isImmuneToDamage = true;
                damageImmunityTime = 1f;
            }
        }
    }

    private void Die()
{

// Check if the player has no more extra lives
        if (ExtraLifeValueToUI.Instance.extraLifeValue <= 0)
        {
            // Load the "GameOver" scene
            SceneManager.LoadSceneAsync("GameOver");
        }

    if (canDie)
    {
        // Reset the player's position to the initial start position of the scene
        GameObject startLocation = GameObject.Find("StartPoint");

        if (startLocation != null)
        {
            transform.position = startLocation.transform.position;
        }
        else
        {
            Debug.LogWarning("StartPoint GameObject not found for respawning.");
        }
            

        // Update the ExtraLife value by taking 1
        ExtraLifeValueToUI.Instance.extraLifeValue -= 1;

        // Reset health and any other necessary variables
         currentHealth = maxHealth;
         
           // Activate the GameObject reference
            activationGameObject.SetActive(true);

            // Start the coroutine to disable the GameObject after a delay
            StartCoroutine(DeactivateGameObjectAfterDelay());

        // Set the cooldown to prevent rapid calls to Die
        canDie = false;
    }
    
}

private System.Collections.IEnumerator DeactivateGameObjectAfterDelay()
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        // Deactivate the GameObject reference
        activationGameObject.SetActive(false);
    }

}
