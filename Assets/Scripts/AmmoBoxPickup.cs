using UnityEngine;

public class AmmoBoxPickup : MonoBehaviour
{
    [Header("CONFIGURACIÓN MUNICIÓN")]
    public int ammoAmount = 30;
    public bool infiniteAmmo = false;
    public float respawnTime = 30f;

    [Header("EFECTOS VISUALES")]
    public GameObject ammoModel;
    public ParticleSystem collectEffect;

    [Header("SONIDO")]
    public AudioClip collectSound;

    private bool isActive = true;

    void Start()
    {
        SetActiveState(true);
    }

    public void PlayerInteract(PlayerShooting player)
    {
        if (!isActive) 
        {
            Debug.Log("❌ CAJA NO ACTIVA");
            return;
        }

        Debug.Log("📦 INTERACTUANDO CON CAJA DE MUNICIÓN");

        // Recargar al jugador
        player.ReloadFromAmmoBox(ammoAmount);

        // Efectos de sonido
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, 1f);
        }

        // Efectos visuales
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        if (!infiniteAmmo)
        {
            SetActiveState(false);
            Invoke("RespawnAmmoBox", respawnTime);
            Debug.Log("⏳ CAJA DESACTIVADA - Reaparecerá en " + respawnTime + " segundos");
        }
        else
        {
            Debug.Log("♾️ CAJA INFINITA - Lista para usar nuevamente");
        }
    }

    void SetActiveState(bool active)
    {
        isActive = active;
        
        if (ammoModel != null)
            ammoModel.SetActive(active);
        
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = active;
    }

    void RespawnAmmoBox()
    {
        SetActiveState(true);
        Debug.Log("🔄 CAJA DE MUNICIÓN REAPARECIDA");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 1.5f);
    }
}