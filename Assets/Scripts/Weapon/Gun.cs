using UnityEngine;
using UnityEngine.Rendering; 

public class Gun : MonoBehaviour
{
    [Header("Disparo")]
    public Transform playerCamera;
    public float shotDistance = 100f;
    public float impactForce = 5f;
    public LayerMask shotMask = ~0; 

    [Header("Efectos visuales")]
    public ParticleSystem shootParticles;
    public GameObject hitEffect; 
    public Light muzzleLight;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Cadencia de disparo")]
    public float shotRate = 0.15f;
    private float nextShotTime = 0f;

    private static Material bloodMaterial;

    private void Awake()
    {
        if (bloodMaterial == null)
        {
            Shader particleShader;
            if (GraphicsSettings.currentRenderPipeline != null)
                particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            else
                particleShader = Shader.Find("Particles/Standard Unlit");

            if (particleShader == null)
            {
                particleShader = Shader.Find("Particles/Standard Unlit");
            }

            bloodMaterial = new Material(particleShader);
            bloodMaterial.color = new Color(0.6f, 0f, 0f);
        }
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
            Shoot();
    }

    private void Shoot()
    {
        if (Time.time < nextShotTime || GameManager.instance.gunAmmo <= 0)
            return;

        nextShotTime = Time.time + shotRate;

        shootParticles?.Play();
        if (muzzleLight != null) StartCoroutine(MuzzleLightEffect());
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, shotDistance, shotMask))
        {
            // Detectar si es un barril con el script Target.cs
            Target target = hit.collider.GetComponentInParent<Target>();
            if (target != null)
            {
                target.Explode();
                GameManager.instance.gunAmmo--;
                return;
            }

            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse);

           AI enemy = hit.collider.GetComponentInParent<AI>();
if (enemy != null)
{
    
    if (enemy.CompareTag("Boss"))
        enemy.TakeDamage(10);
    else
        enemy.TakeDamage(1);

    CreateBloodParticle(hit.point, Quaternion.LookRotation(hit.normal));
}

            else
            {
                if (hitEffect != null)
                {
                    GameObject go = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(go, 3f);
                }
            }
        }

        GameManager.instance.gunAmmo--;
    }

    private void CreateBloodParticle(Vector3 position, Quaternion rotation)
    {
        GameObject bloodPS = new GameObject("BloodEffect");
        var ps = bloodPS.AddComponent<ParticleSystem>();

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.25f;
        main.startLifetime = 0.15f;
        main.startSpeed = 4f;
        main.startSize = 0.12f;
        main.loop = false;
        main.maxParticles = 30;
        main.startColor = new Color(0.6f, 0f, 0f);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 12) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.04f;

        bloodPS.transform.position = position;
        bloodPS.transform.rotation = rotation;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (bloodMaterial != null)
            renderer.material = bloodMaterial;

        ps.Play();
        Destroy(bloodPS, main.duration + main.startLifetime.constant + 0.1f);
    }

    private System.Collections.IEnumerator MuzzleLightEffect()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
    }
}

