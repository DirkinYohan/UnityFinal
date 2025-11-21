using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [Header("DISPARO")]
    public float fireRate = 0.2f;
    public int damagePerShot = 10;

    [Header("MUNICIÓN")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public int ammoPerReload = 30;
    public float reloadTime = 1.5f;

    [Header("UI")]
    public GameObject ammoText;
    public GameObject reloadPrompt;

    [Header("SONIDO")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;
    public AudioSource audioSource;

    [Header("EFECTOS VISUALES")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("PUNTO DE MIRA")]
    public Transform crosshair;
    public Camera playerCamera;

    [Header("LAYER MASK")]
    public LayerMask enemyLayerMask = 1;

    [Header("CAJAS MOVIBLES")]
    public float fuerzaCaja = 15f;

    [Header("DETECCIÓN CAJAS")]
    public float ammoBoxDetectionRange = 3f;

    private float nextFireTime = 0f;
    private bool isReloading = false;
    private AmmoBoxPickup nearbyAmmoBox;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        currentAmmo = maxAmmo;
        
        Debug.Log("🎮 INICIANDO SISTEMA DE MUNICIÓN");
        Debug.Log("🔫 Munición inicial: " + currentAmmo);
        Debug.Log("📺 AmmoText asignado: " + (ammoText != null));
        
        if (ammoText != null)
        {
            Debug.Log("📺 Nombre del objeto AmmoText: " + ammoText.name);
        }
        
        UpdateAmmoUI();
        
        if (reloadPrompt != null)
            reloadPrompt.SetActive(false);
    }

    void Update()
    {
        // Disparar
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                if (emptySound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(emptySound);
                }
                nextFireTime = Time.time + 0.5f;
                Debug.Log("⚠️ NO HAY MUNICIÓN");
            }
        }

        // Detectar cajas cercanas
        FindNearbyAmmoBox();

        // Recargar con E si hay caja cerca
        if (Input.GetKeyDown(KeyCode.E) && !isReloading && nearbyAmmoBox != null)
        {
            Debug.Log("🎯 PRESIONANDO E - RECARGANDO DESDE CAJA");
            nearbyAmmoBox.PlayerInteract(this);
        }

        // Recargar manual con R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }

        UpdateReloadPrompt();
    }

    void FindNearbyAmmoBox()
    {
        AmmoBoxPickup[] allAmmoBoxes = FindObjectsOfType<AmmoBoxPickup>();
        nearbyAmmoBox = null;

        float closestDistance = Mathf.Infinity;

        foreach (AmmoBoxPickup ammoBox in allAmmoBoxes)
        {
            if (ammoBox.isActiveAndEnabled)
            {
                float distance = Vector3.Distance(transform.position, ammoBox.transform.position);
                
                if (distance <= ammoBoxDetectionRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    nearbyAmmoBox = ammoBox;
                }
            }
        }

        // Debug visual
        if (nearbyAmmoBox != null)
        {
            Debug.DrawLine(transform.position, nearbyAmmoBox.transform.position, Color.green);
        }
    }

    void Shoot()
    {
        Debug.Log("🔫 DISPARO - Munición antes: " + currentAmmo);

        currentAmmo--;
        
        Debug.Log("🔫 DISPARO - Munición después: " + currentAmmo);
        UpdateAmmoUI();

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        ShootTowardsCrosshair();
    }

    void ShootTowardsCrosshair()
    {
        if (playerCamera == null) return;

        Vector3 shootDirection;
        
        if (crosshair != null)
        {
            shootDirection = (crosshair.position - transform.position).normalized;
        }
        else
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            shootDirection = ray.direction;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, shootDirection, out hit, 100f, enemyLayerMask))
        {
            Debug.Log("Impacto en: " + hit.collider.name);
            
            CheckEnemyHit(hit.collider.gameObject);
            CheckBarrelHit(hit.collider.gameObject);
            CheckBoxHit(hit.collider.gameObject, shootDirection);
            
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        else
        {
            Debug.DrawRay(playerCamera.transform.position, shootDirection * 100f, Color.red, 1f);
        }
    }

    void StartReload()
    {
        if (isReloading) return;

        isReloading = true;
        Debug.Log("🔄 INICIANDO RECARGA MANUAL");

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToAdd = Mathf.Min(ammoNeeded, ammoPerReload);
        
        currentAmmo += ammoToAdd;
        isReloading = false;
        
        UpdateAmmoUI();
        Debug.Log("✅ RECARGA MANUAL COMPLETADA - Munición: " + currentAmmo);
    }

    public void ReloadFromAmmoBox(int ammoAmount)
    {
        int ammoBefore = currentAmmo;
        currentAmmo = Mathf.Min(currentAmmo + ammoAmount, maxAmmo);
        int ammoAdded = currentAmmo - ammoBefore;
        
        UpdateAmmoUI();
        Debug.Log($"📦 RECARGADO DESDE CAJA: +{ammoAdded} balas - Total: {currentAmmo}/{maxAmmo}");
    }

    void UpdateAmmoUI()
    {
        Debug.Log("🔄 ACTUALIZANDO UI - Munición actual: " + currentAmmo);
        
        if (ammoText != null)
        {
            Debug.Log("✅ AmmoText no es null");
            
            // Intentar con Text Legacy (Unity UI)
            Text legacyText = ammoText.GetComponent<Text>();
            if (legacyText != null)
            {
                Debug.Log("✅ Encontrado componente Text Legacy");
                legacyText.text = $"BALAS: {currentAmmo}/{maxAmmo}";
                Debug.Log("✅ Texto actualizado: " + legacyText.text);
                return;
            }
            else
            {
                Debug.Log("❌ No se encontró componente Text Legacy");
            }

            // Intentar con TextMeshPro
            try
            {
                TMPro.TextMeshProUGUI tmpText = ammoText.GetComponent<TMPro.TextMeshProUGUI>();
                if (tmpText != null)
                {
                    Debug.Log("✅ Encontrado componente TextMeshPro");
                    tmpText.text = $"BALAS: {currentAmmo}/{maxAmmo}";
                    Debug.Log("✅ Texto actualizado: " + tmpText.text);
                    return;
                }
                else
                {
                    Debug.Log("❌ No se encontró componente TextMeshPro");
                }
            }
            catch (System.Exception)
            {
                Debug.Log("ℹ️ TextMeshPro no está disponible");
            }

            Debug.Log("❌ No se encontró ningún componente de texto válido");
        }
        else
        {
            Debug.LogError("❌ AmmoText es null - No hay referencia al objeto de UI");
        }
    }

    void UpdateReloadPrompt()
    {
        if (reloadPrompt != null)
        {
            bool showPrompt = (nearbyAmmoBox != null) && (currentAmmo < maxAmmo);
            reloadPrompt.SetActive(showPrompt);
            
            if (showPrompt)
            {
                // Actualizar texto si es necesario
                Text legacyText = reloadPrompt.GetComponent<Text>();
                if (legacyText != null)
                {
                    legacyText.text = "Presiona E para recargar";
                    return;
                }

                try
                {
                    TMPro.TextMeshProUGUI tmpText = reloadPrompt.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = "Presiona E para recargar";
                        return;
                    }
                }
                catch (System.Exception)
                {
                    // TextMeshPro no disponible
                }
            }
        }
    }

    void CheckEnemyHit(GameObject hitObject)
    {
        WarrokEnemy warrokEnemy = hitObject.GetComponent<WarrokEnemy>();
        if (warrokEnemy == null)
        {
            warrokEnemy = hitObject.GetComponentInParent<WarrokEnemy>();
        }

        if (warrokEnemy != null && !warrokEnemy.IsDead())
        {
            Debug.Log("🎯 WARROK ENEMY IMPACTADO - Aplicando " + damagePerShot + " de daño");
            warrokEnemy.TakeDamage(damagePerShot);
            
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hitObject.transform.position + Vector3.up, Quaternion.identity);
            }
            return;
        }

        MutantEnemy mutantEnemy = hitObject.GetComponent<MutantEnemy>();
        if (mutantEnemy == null)
        {
            mutantEnemy = hitObject.GetComponentInParent<MutantEnemy>();
        }

        if (mutantEnemy != null)
        {
            Debug.Log("🎯 MUTANT ENEMY IMPACTADO - Aplicando " + damagePerShot + " de daño");
            mutantEnemy.RecibirDano(damagePerShot);
            
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hitObject.transform.position + Vector3.up, Quaternion.identity);
            }
        }
    }

    void CheckBarrelHit(GameObject hitObject)
    {
        ExplosiveBarrel barrel = hitObject.GetComponent<ExplosiveBarrel>();
        if (barrel == null)
        {
            barrel = hitObject.GetComponentInParent<ExplosiveBarrel>();
        }

        if (barrel != null)
        {
            Debug.Log("🎯 BARRIL IMPACTADO - Aplicando " + damagePerShot + " de daño");
            barrel.TakeDamage(damagePerShot);
            
            if (impactEffect != null)
            {
                Instantiate(impactEffect, hitObject.transform.position, Quaternion.identity);
            }
        }
    }

    void CheckBoxHit(GameObject hitObject, Vector3 shootDirection)
    {
        MovableBox movableBox = hitObject.GetComponent<MovableBox>();
        if (movableBox == null)
        {
            movableBox = hitObject.GetComponentInParent<MovableBox>();
        }

        if (movableBox != null)
        {
            Debug.Log("📦 CAJA MOVIBLE IMPACTADA");
            movableBox.MoverCaja(shootDirection, fuerzaCaja);
        }
    }

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    // Dibujar rango de detección en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ammoBoxDetectionRange);
    }

    void OnValidate()
    {
        if (enemyLayerMask == 0)
            enemyLayerMask = 1;
    }
}