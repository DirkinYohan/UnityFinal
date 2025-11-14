using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("UI")]
    public TMP_Text ammoText;
    public TMP_Text healthText;
    public TMP_Text grenadesText;

    public GameObject gameOverPanel; // ← Aquí debes arrastrar tu panel

    public int gunAmmo = 100;
    public int health = 100;
    public int grenades = 3;

    private bool isDead = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // oculto inicialmente
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
        grenadesText.text = grenades.ToString();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            health = 0;
            StartCoroutine(GameOverRoutine());
        }
    }

    IEnumerator GameOverRoutine()
    {
        isDead = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true); // mostrar GAME OVER

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reinicia nivel
    }
}
