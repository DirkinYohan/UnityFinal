using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject explosionEffect;

    private void OnDestroy()
    {
        if (GameLevelManager.Instance != null && gameObject.scene.isLoaded)
        {
            GameLevelManager.Instance.TargetDestroyed();
        }
    }

    public void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        // ✅ Sumar 100 puntos por barril
        if (GameLevelManager.Instance != null)
        {
            GameLevelManager.Instance.AddScore(100);
        }

        Destroy(gameObject);
    }
}
