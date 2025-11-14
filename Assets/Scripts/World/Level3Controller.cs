using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level3Controller : MonoBehaviour
{
    public GameObject winPanel;
    public TextMeshProUGUI tiempo1Text;
    public TextMeshProUGUI tiempo2Text;
    public TextMeshProUGUI tiempo3Text;

    private bool nivelCompleto = false;

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void CompletarNivel3()
    {
        if (nivelCompleto) return;
        nivelCompleto = true;

        // Registrar tiempo del nivel 3
        LevelManager.instance.tiempoNivel3 = Time.timeSinceLevelLoad;

        // Mostrar tiempos
        tiempo1Text.text = $"Tiempo nivel 1: {LevelManager.instance.tiempoNivel1:F2} s";
        tiempo2Text.text = $"Tiempo nivel 2: {LevelManager.instance.tiempoNivel2:F2} s";
        tiempo3Text.text = $"Tiempo nivel 3: {LevelManager.instance.tiempoNivel3:F2} s";

        winPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;

        LevelManager.instance.tiempoNivel1 = 0;
        LevelManager.instance.tiempoNivel2 = 0;
        LevelManager.instance.tiempoNivel3 = 0;

        SceneManager.LoadScene("Aeropuerto"); // Primer nivel
    }
}

