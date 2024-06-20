using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        // Apply the damage to the enemy's health
        currentHealth -= damageAmount;
        Debug.Log("Enemy took " + damageAmount + " damage!");

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

  private void Die()
{
    // Deactivate the enemy GameObject instead of destroying it
    gameObject.SetActive(false);
}
}
