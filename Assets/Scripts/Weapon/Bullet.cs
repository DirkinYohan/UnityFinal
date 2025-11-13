using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1; // 💥 Daño que hace cada bala
    public float lifeTime = 3f; // ⏱ Tiempo de vida antes de destruirse

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObj = collision.gameObject;

        // 💀 Si el objeto está en el layer 11
        if (hitObj.layer == 11)
        {
            // Buscar el componente AI (en este objeto o en sus padres)
            AI enemy = hitObj.GetComponentInParent<AI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Resta vida
            }
            else
            {
                // Si no tiene vida, destruir directamente
                Destroy(hitObj);
            }
        }

        // 💣 Destruir la bala siempre tras impactar
        Destroy(gameObject);
    }
}
