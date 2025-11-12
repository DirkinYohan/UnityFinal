using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WarrokEnemy : MonoBehaviour
{
    [Header("OBJETIVO")]
    public Transform objetivo;
    
    [Header("VELOCIDAD")]
    public float moveSpeed = 3f;
    
    [Header("IA")]
    public NavMeshAgent navMeshAgent;
    public float detectionRange = 10f;
    public float rotationSpeed = 5f;
    public bool smoothRotation = true;
    
    [Header("ATAQUE SOUND")]
    public AudioClip ataqueSound;
    public AudioSource audioSourceAtaque;
    
    [Header("MUERTE SOUND")]
    public AudioClip muerteSound;
    public AudioSource audioSourceMuerte;
    
    [Header("HEALTH")]
    public int health = 100;
    public int maxHealth = 100;
    
    [Header("COMBATE")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    [Header("ANIMATION")]
    public Animator animator;
    
    private EnemyState currentState = EnemyState.Idle;
    private bool isPlayerDetected = false;
    private float lastAttackTime = 0f;
    private Vector3 initialPosition;
    private bool isDead = false;
    
    private enum EnemyState
    {
        Idle,
        Run,
        Attack,
        Death,
        Exit
    }
    
    void Start()
    {
        InitializeComponents();
        health = maxHealth;
        initialPosition = transform.position;
        
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.stoppingDistance = attackRange - 0.5f;
        }
    }
    
    void InitializeComponents()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
            
        if (objetivo == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                objetivo = playerObj.transform;
        }
        
        SetupAudioSources();
    }
    
    void SetupAudioSources()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            if (audioSourceAtaque == null)
                audioSourceAtaque = audioSources[0];
            if (audioSourceMuerte == null)
                audioSourceMuerte = audioSources[1];
        }
        else if (audioSources.Length == 1)
        {
            if (audioSourceAtaque == null)
                audioSourceAtaque = audioSources[0];
        }
    }
    
    void Update()
    {
        if (isDead || currentState == EnemyState.Death || currentState == EnemyState.Exit)
            return;
            
        CheckPlayerDetection();
        StateMachine();
        UpdateAnimator();
    }
    
    void CheckPlayerDetection()
    {
        if (objetivo == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, objetivo.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            isPlayerDetected = true;
            
            if (smoothRotation)
            {
                Vector3 direction = (objetivo.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            isPlayerDetected = false;
        }
    }
    
    void StateMachine()
    {
        if (objetivo == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, objetivo.position);
        
        switch (currentState)
        {
            case EnemyState.Idle:
                if (isPlayerDetected)
                {
                    if (distanceToPlayer <= attackRange)
                    {
                        ChangeState(EnemyState.Attack);
                    }
                    else
                    {
                        ChangeState(EnemyState.Run);
                    }
                }
                break;
                
            case EnemyState.Run:
                if (!isPlayerDetected)
                {
                    ChangeState(EnemyState.Idle);
                    StopNavMeshAgent();
                }
                else if (distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                    StopNavMeshAgent();
                }
                else
                {
                    MoveTowardsPlayer();
                }
                break;
                
            case EnemyState.Attack:
                if (!isPlayerDetected)
                {
                    ChangeState(EnemyState.Idle);
                }
                else if (distanceToPlayer > attackRange)
                {
                    ChangeState(EnemyState.Run);
                }
                else
                {
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        PerformAttack();
                        lastAttackTime = Time.time;
                    }
                }
                break;
        }
    }
    
    void MoveTowardsPlayer()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(objetivo.position);
        }
        else
        {
            Vector3 direction = (objetivo.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
    
    void StopNavMeshAgent()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.isStopped = true;
        }
    }
    
    void ChangeState(EnemyState newState)
    {
        currentState = newState;
        
        if (newState == EnemyState.Death)
        {
            OnDeath();
        }
    }
    
    void UpdateAnimator()
    {
        animator.SetBool("IsIdle", currentState == EnemyState.Idle);
        animator.SetBool("IsRunning", currentState == EnemyState.Run);
        animator.SetBool("IsAttacking", currentState == EnemyState.Attack);
        animator.SetBool("IsDead", currentState == EnemyState.Death);
    }
    
    void PerformAttack()
    {
        Debug.Log("Warrok ataca al jugador!");
        PlayAttackSound();
        animator.SetTrigger("Attack");
        
        if (objetivo != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, objetivo.position);
            if (distanceToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = objetivo.GetComponent<PlayerHealth>();
                if (playerHealth != null) 
                    playerHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    void PlayAttackSound()
    {
        if (audioSourceAtaque != null && ataqueSound != null)
        {
            audioSourceAtaque.clip = ataqueSound;
            audioSourceAtaque.Play();
        }
    }
    
    void PlayDeathSound()
    {
        if (audioSourceMuerte != null && muerteSound != null)
        {
            audioSourceMuerte.clip = muerteSound;
            audioSourceMuerte.Play();
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        
        Debug.Log($"Warrok recibe {damage} de daño. Salud restante: {health}");
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        ChangeState(EnemyState.Death);
    }
    
    void OnDeath()
    {
        PlayDeathSound();
        StopNavMeshAgent();
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;
            
        StartCoroutine(DestroyAfterDeath());
    }
    
    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
    
    // ✅ MÉTODOS PÚBLICOS PARA LEVEL MANAGER
    public bool IsDead()
    {
        return isDead;
    }
    
    public bool IsAlive()
    {
        return !isDead && health > 0;
    }
    
    public int GetHealth()
    {
        return health;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}