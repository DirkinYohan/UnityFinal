using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgression : MonoBehaviour
{
    public static LevelProgression instance;

    [Header("Configuración de niveles")]
    public string nivel2Nombre = "Desierto";  // nombre exacto en Build Settings
    public int enemigosParaPasar = 6;         // cantidad necesaria para pasar al siguiente nivel

    private int enemigosEliminados = 0;

    private void Awake()
    {
        // Singleton para mantener una sola instancia entre escenas
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 👉 Llamar este método cada vez que muere un enemigo
    public void RegistrarMuerteEnemigo()
    {
        enemigosEliminados++;
        Debug.Log($"☠️ Enemigo eliminado ({enemigosEliminados}/{enemigosParaPasar})");

        string escenaActual = SceneManager.GetActiveScene().name;

        // Solo contar progresión si estamos en el Desierto
        if (escenaActual == nivel2Nombre && enemigosEliminados >= enemigosParaPasar)
        {
            Debug.Log("Objetivo del nivel 2 cumplido. Pasando a la siguiente escena...");
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentScene + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
            enemigosEliminados = 0; // reiniciar conteo
        }
        else
        {
            Debug.Log("No hay más niveles disponibles.");
        }
    }
}
