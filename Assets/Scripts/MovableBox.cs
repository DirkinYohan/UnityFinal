using UnityEngine;

public class MovableBox : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float fuerzaMovimiento = 10f;
    
    private Rigidbody rb;

    void Start()
    {
        // Obtener el Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            // Si no tiene Rigidbody, agregar uno automáticamente
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    // Método simple para mover la caja cuando le disparan
    public void MoverCaja(Vector3 direccion, float fuerza)
    {
        // Usar la fuerza proporcionada o la fuerza por defecto
        float fuerzaAplicar = fuerza > 0f ? fuerza : fuerzaMovimiento;
        
        // Aplicar fuerza en la dirección del disparo
        rb.AddForce(direccion * fuerzaAplicar, ForceMode.Impulse);
    }
}