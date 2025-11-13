using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    [Header("Patrol")]
    public Transform[] destinations;
    public float distanceToFollowPath = 2f;
    private int i = 0;

    [Header("Follow Player")]
    public bool followPlayer = true;
    private GameObject player;
    private float distanceToPlayer;
    public float distanceToFollowPlayer = 10f;

    [Header("Attack")]
    public float attackRange = 4;
    public float attackCooldown = 1.5f;
    private bool canAttack = true;

    [Header("Speed Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Health")]
    public int health = 5;
    public bool isDead = false;

    [Header("Boss Settings")]
    public bool isBoss = false; // ✅ Marcar en el inspector si es el jefe
    public int bossHealth = 100; // vida máxima del jefe

    [Header("Animation")]
    public Animator animator;

    void Start()
    {
        gameObject.layer = 11;

        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        player = FindObjectOfType<PlayerMovementL>()?.gameObject;

        // Si es jefe, asignar su vida especial
        if (isBoss)
            health = bossHealth;

        if (destinations.Length > 0)
        {
            navMeshAgent.destination = destinations[0].position;
            navMeshAgent.speed = patrolSpeed;
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            return;
        }

        if (followPlayer && distanceToPlayer <= distanceToFollowPlayer)
        {
            FollowPlayerAI();
            return;
        }

        EnemyPath();
    }

    public void EnemyPath()
    {
        if (destinations.Length == 0) return;

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.destination = destinations[i].position;

        animator.SetBool("isRunning", false);

        if (Vector3.Distance(transform.position, destinations[i].position) < distanceToFollowPath)
            i = (i + 1) % destinations.Length;
    }

    public void FollowPlayerAI()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = chaseSpeed;
        navMeshAgent.destination = player.transform.position;

        animator.SetBool("isRunning", true);
    }

    void AttackPlayer()
    {
        navMeshAgent.isStopped = true;
        animator.SetBool("isRunning", false);

        if (canAttack)
        {
            canAttack = false;
            animator.SetTrigger("Attack");
            StartCoroutine(AttackCooldownTimer());
        }
    }

    public void DealDamage()
    {
        if (isDead) return;

        float realDistance = Vector3.Distance(transform.position, player.transform.position);

        if (realDistance <= attackRange)
        {
            GameManager.instance.health -= 10;

            if (GameManager.instance.health <= 0)
                Invoke(nameof(RestartLevel), 0.5f);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator AttackCooldownTimer()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // --- DAÑO Y MUERTE ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // ✅ Si es jefe, el daño de las balas es 10
        if (isBoss)
            damage = 10;

        health -= damage;

        if (health <= 0)
            Die();
    }

  public void GrenadeImpact()
{
    if (isDead) return;

    if (CompareTag("Boss"))
        TakeDamage(30);
    else
        TakeDamage(999);
}


   void Die()
{
    isDead = true;
    navMeshAgent.enabled = false;
    animator.SetBool("isDead", true);

    if (CompareTag("Boss"))
    {
        // 🧟‍♂️ Si es el jefe
        if (GameLevelManager.Instance != null)
            GameLevelManager.Instance.BossDefeated();
    }
    else
    {
        // 👾 Enemigos normales suman puntos
        if (GameLevelManager.Instance != null)
            GameLevelManager.Instance.AddScore(50);
    }

    Destroy(gameObject, 3f);
}

}
