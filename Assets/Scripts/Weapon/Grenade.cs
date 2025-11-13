using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion settings")]
    public float delay = 3f;
    public float radius = 5f;
    public float explosionForce = 70f;

    [Header("FX")]
    public GameObject explosionEffect;
    private AudioSource audioSource;
    public AudioClip explosionSound;

    float countdown;
    bool exploded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f && !exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        // Efecto visual
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Detectar objetos dentro del radio
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in colliders)
        {
            // Si encuentra un enemigo con IA, aplicar daño o muerte directa
            AI ai = col.GetComponent<AI>();
            if (ai != null)
            {
                ai.GrenadeImpact();
            }

            // Si tiene rigidbody, aplicarle empuje
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, radius);
            }
        }

        // Sonido de explosión
        if (audioSource != null && explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Desactivar el objeto para evitar colisiones extra
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;

        // Destruir la granada después de un tiempo
        Destroy(gameObject, 1f);
    }
}
