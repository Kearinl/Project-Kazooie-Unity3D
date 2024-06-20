using UnityEngine;

using System.Collections; // Add this line to include IEnumerator

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f; // The range of the attack hitbox
    public int attackDamage = 10; // The amount of damage to apply to enemies or objects

    public LayerMask attackLayerMask; // The layer mask to specify which objects the attack can hit

    private Animator _animator;
    public AudioClip attackAudioClip; // Reference to the audio clip for playing attack sound


    public float attackCooldown = 2f; // The cooldown duration in seconds
    private bool isAttackOnCooldown = false; // Flag to track if the attack is on cooldown

     private int attackAudioCounter = 0; // Counter for the number of times the audio clip is played

    private void Start()
    {
        // Get the Animator component attached to the same GameObject
        _animator = GetComponent<Animator>();
    }

     public void PerformAttack()
    {
        // Don't perform another attack if already attacking or on cooldown
        if (_animator.GetBool("isAttacking") || isAttackOnCooldown)
        {
            return;
        }

        // Cast a sphere in front of the character to detect hit objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward, attackRange, attackLayerMask);

        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the hit object has a component that can take damage (you can define your own interface for this)
            IDamageable damageableObject = hitCollider.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                // Apply damage to the object
                damageableObject.TakeDamage(attackDamage);
            }
        }

        // Trigger the attack animation
        _animator.SetTrigger("Attack");

        // Play the attack audio clip multiple times and stop after the specified duration
        StartCoroutine(PlayAttackAudioWithCooldown(3, attackCooldown));

        // Put the attack on cooldown
        isAttackOnCooldown = true;
        Invoke("ResetAttackCooldown", attackCooldown);
    }

    private void ResetAttackCooldown()
    {
        // Reset the attack cooldown flag after the cooldown duration
        isAttackOnCooldown = false;
    }

   private IEnumerator PlayAttackAudioWithCooldown(int numPlays, float cooldown)
    {
        // Play the attack audio clip immediately for the first time
        if (attackAudioCounter == 0 && attackAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(attackAudioClip, transform.position);
            attackAudioCounter++;
        }

        for (int i = 1; i < numPlays; i++)
        {
            // Play the attack audio clip
            if (attackAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(attackAudioClip, transform.position);
                attackAudioCounter++;
            }
        }

        // Add the return statement here to fix the error
        yield return null;
    }
}
