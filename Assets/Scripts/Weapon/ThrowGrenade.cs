using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{
    public GameObject grenadePrefab;
    public float throwForce = 500f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Throw();
        }
    }

    private void Throw()
    {
        // ✅ Si no tienes granadas, no lanza nada
        if (GameManager.instance.grenades <= 0)
            return;

        // ✅ Instanciar granada
        GameObject newGrenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        newGrenade.GetComponent<Rigidbody>().AddForce(transform.forward * throwForce);

        // ✅ Restar 1 granada SOLO AQUÍ
        GameManager.instance.grenades--;
    }
}
