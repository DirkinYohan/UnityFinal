using UnityEngine;
using TMPro;

public class BossHealthDisplay : MonoBehaviour
{
    public AI boss;                // referencia al jefe
    public TMP_Text healthText;    // texto que mostrará la vida

    void Update()
    {
        if (boss == null || boss.isDead || healthText == null)
        {
            if (healthText != null)
                healthText.text = "";
            return;
        }

        // Mostrar vida actual del jefe
        healthText.text = $"{boss.health}/100";
    }
}
