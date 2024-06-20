using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float chaseRange = 10f; // The range at which the enemy starts chasing the player
    public float attackRange = 1.5f; // The range at which the enemy attacks the player
    public int attackDamage = 10; // The amount of damage the enemy's attack will do
    public float wanderRadius = 5f; // The radius within which the enemy will wander when not chasing
    public float movementSpeed = 3f; // The movement speed of the enemy

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private bool isChasing;
    private bool isWithinAttackRange;

    private Vector3 wanderTarget; // The position to which the enemy will wander
    private bool isWandering;
    
    private float wanderTimer; // The time the enemy will wander before picking a new wandering target
    public float minWanderTime = 3f; // The minimum time the enemy will wander
    public float maxWanderTime = 10f; // The maximum time the enemy will wander

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        // Set the movement speed of the NavMeshAgent
        navMeshAgent.speed = movementSpeed;
    }

   private void Update()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    if (distanceToPlayer <= chaseRange)
    {
        // Start chasing the player
        navMeshAgent.SetDestination(player.position);
        isChasing = true;
        isWandering = false;
    }
    else
    {
        // Stop chasing if the player is out of range
        isChasing = false;
    }

    if (!isChasing)
    {
        // If not chasing, keep wandering
        Wander();
    }

    if (distanceToPlayer <= attackRange)
    {
        // Within attack range, stop moving and perform the attack
        navMeshAgent.isStopped = true;

        if (!isWithinAttackRange)
        {
            // Perform the attack once when entering attack range
            AttackPlayer();
        }
        isWithinAttackRange = true;
    }
    else
    {
        // Outside attack range, resume moving and stop attacking
        navMeshAgent.isStopped = false;
        isWithinAttackRange = false;
    }

    // Set the "IsAttacking" parameter based on the current state
    animator.SetBool("IsAttacking", isChasing);

    // Update the animator parameter for movement
    animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
}


    private void AttackPlayer()
    {
        // Play attack animation if you have one in the animator

        // Perform the attack logic
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage); // Apply damage to the player
        }
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            // Generate a random point within the specified wander radius
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1);

            // Set the new wandering target
            wanderTarget = navHit.position;
            navMeshAgent.SetDestination(wanderTarget);

            // Set a new random wander timer
            wanderTimer = Random.Range(minWanderTime, maxWanderTime);
            isWandering = true;
        }
    }
}
