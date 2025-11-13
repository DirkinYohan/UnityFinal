using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractions : MonoBehaviour
{
    public TextMeshProUGUI pressEText;
    private AmmoBox ammoBoxInRange;

    private void Start()
    {
        pressEText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (ammoBoxInRange != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.instance.gunAmmo += ammoBoxInRange.ammo;
                Destroy(ammoBoxInRange.gameObject);
                ammoBoxInRange = null;
                pressEText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GunAmmo"))
        {
            ammoBoxInRange = other.GetComponent<AmmoBox>();
            pressEText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GunAmmo"))
        {
            ammoBoxInRange = null;
            pressEText.gameObject.SetActive(false);
        }
    }
}
