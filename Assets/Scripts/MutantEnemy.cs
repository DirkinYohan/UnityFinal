using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantEnemy : MonoBehaviour
{
    public Transform Objetivo;
    public float Velocidad;
    public NavMeshAgent IA;
    
    // Variables para los sonidos
    public AudioClip AtaqueSound;
    public AudioClip MuerteSound;
    public AudioSource AudioSourceAtaque;
    public AudioSource AudioSourceMuerte;
    
    // Variables de salud añadidas
    public float health = 100f;
    public float maxHealth = 100f;

    // Variables de ataque
    public float attackDamage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    // Variables para el giro
    public float rotationSpeed = 5f; // Velocidad de rotación hacia el jugador
    public bool smoothRotation = true; // Si queremos rotación suave o instantánea

    // Referencia al Animator
    private Animator animator;
    
    // Parámetros del Animator
    private readonly int velocidadHash = Animator.StringToHash("Velocidad");
    private readonly int atacandoHash = Animator.StringToHash("Atacando");
    private readonly int muertoHash = Animator.StringToHash("Muerto");
    private readonly int recibiendoDanoHash = Animator.StringToHash("RecibiendoDano");

    private bool estaMuerto = false;
    private bool estaAtacando = false;

    void Start()
    {
        if (AudioSourceAtaque == null)
            AudioSourceAtaque = GetComponent<AudioSource>();
            
        // Obtener referencia al Animator
        animator = GetComponent<Animator>();
        
        // Inicializar salud
        health = maxHealth;
    }

    void Update()
    {
        if (estaMuerto) return;
        
        IA.speed = Velocidad;
        IA.SetDestination(Objetivo.position);
        
        if (animator != null)
        {
            animator.SetFloat(velocidadHash, IA.velocity.magnitude);
        }
        
        // Girar hacia el jugador
        RotateTowardsPlayer();
        
        // Lógica de ataque MEJORADA
        float distanceToPlayer = Vector3.Distance(transform.position, Objetivo.position);
        
        if (distanceToPlayer <= attackRange)
        {
            if (!estaAtacando && Time.time >= lastAttackTime + attackCooldown)
            {
                Atacar();
            }
            
            // Aplicar daño cuando esté atacando
            if (estaAtacando && distanceToPlayer <= attackRange)
            {
                TryApplyDamage();
            }
        }
        else
        {
            estaAtacando = false;
            if (animator != null)
            {
                animator.SetBool(atacandoHash, false);
            }
        }
    }
    
    // NUEVO MÉTODO para girar hacia el jugador
    private void RotateTowardsPlayer()
    {
        if (Objetivo == null) return;
        
        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = Objetivo.position - transform.position;
        directionToPlayer.y = 0; // Mantener la rotación solo en el eje Y
        
        // Si hay dirección válida
        if (directionToPlayer != Vector3.zero)
        {
            if (smoothRotation)
            {
                // Rotación suave usando Quaternion.Slerp
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Rotación instantánea
                transform.rotation = Quaternion.LookRotation(directionToPlayer);
            }
        }
    }
    
    // NUEVO MÉTODO para aplicar daño
    private void TryApplyDamage()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PlayerHealth playerHealth = Objetivo.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                lastAttackTime = Time.time;
                Debug.Log("Daño aplicado al jugador: " + attackDamage);
            }
        }
    }
    
    public void Atacar()
    {
        estaAtacando = true;
        
        // Activar parámetro de ataque
        if (animator != null)
        {
            animator.SetBool(atacandoHash, true);
        }
        
        if (AtaqueSound != null && AudioSourceAtaque != null && !AudioSourceAtaque.isPlaying)
        {
            AudioSourceAtaque.clip = AtaqueSound;
            AudioSourceAtaque.Play();
        }
    }
    
    // Método para ser llamado desde la animación de ataque (evento de animación)
    public void FinAtaque()
    {
        estaAtacando = false;
        if (animator != null)
        {
            animator.SetBool(atacandoHash, false);
        }
    }
    
    public void Morir()
    {
        if (estaMuerto) return;
        
        estaMuerto = true;
        IA.isStopped = true;
        
        // Activar parámetro de muerte
        if (animator != null)
        {
            animator.SetBool(muertoHash, true);
        }
        
        if (MuerteSound != null && AudioSourceMuerte != null)
        {
            AudioSourceMuerte.clip = MuerteSound;
            AudioSourceMuerte.Play();
        }
        
        StartCoroutine(DestruirDespuesDeSonido());
        Debug.Log("Enemigo muerto!");
    }

    private IEnumerator DestruirDespuesDeSonido()
    {
        if (AudioSourceMuerte != null && MuerteSound != null)
        {
            yield return new WaitForSeconds(MuerteSound.length);
        }
        
        Destroy(gameObject);
    }
    
    public void RecibirDano(int cantidadDano)
    {
        if (estaMuerto) return;
        
        health -= cantidadDano;
        
        // Trigger para animación de recibir daño
        if (animator != null)
        {
            animator.SetTrigger(recibiendoDanoHash);
        }
        
        if (health <= 0)
        {
            health = 0;
            Morir();
        }
    }
    
    // Nuevo método para obtener la salud actual (útil para la barra de salud)
    public float GetHealth()
    {
        return health;
    }
    
    // Nuevo método para obtener la salud máxima (útil para la barra de salud)
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    // Método para verificar si está muerto (útil para otros scripts)
    public bool EstaMuerto()
    {
        return estaMuerto;
    }
}