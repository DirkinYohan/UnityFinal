using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance;

    public GameObject winPanel;
    public TMPro.TMP_Text winText;
    public TMPro.TMP_Text scoreText;

    private int score = 0;
    private bool levelCompleted = false;
    private bool bossDefeated = false; // 🧟‍♂️ Nuevo

    [Header("Configuración de puntos")]
    public int puntosObjetivo = 500;

    private void Awake()
    {
        Instance = this;

        if (winPanel != null)
            winPanel.SetActive(false);

        UpdateScoreUI();
    }

    // ➕ Agregar puntos
    public void AddScore(int amount)
    {
        if (levelCompleted) return;

        score += amount;
        UpdateScoreUI();

        CheckWinCondition(); // 👈 verificamos si cumple las dos condiciones
    }

    // 🧟‍♂️ Llamado cuando el Boss muere
    public void BossDefeated()
    {
        bossDefeated = true;
        Debug.Log("Boss derrotado!");
        CheckWinCondition();
    }

    // ⚙️ Verificar ambas condiciones
    private void CheckWinCondition()
    {
        if (score >= puntosObjetivo && bossDefeated)
        {
            WinGame();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Puntos: " + score.ToString();
    }

    public void TargetDestroyed()
    {
        // Mantiene compatibilidad si lo usas con barriles antiguos
    }

    private void WinGame()
    {
        if (levelCompleted) return;
        levelCompleted = true;

        Time.timeScale = 0f;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = "¡TU GANASTE!\nHecho por Luis Cajigas, Dirkin Ojeda, Jonar Andrés";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 🔁 Botón de repetir nivel
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
