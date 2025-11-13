using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text healthText;
    public TMP_Text grenadesText;

    public int gunAmmo = 100;
    public int health = 100;
    public int grenades = 3;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
        grenadesText.text = grenades.ToString();   // ✅ Actualiza el UI correctamente
    }
}
