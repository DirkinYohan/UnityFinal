using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    [Header("Configuración Nivel 2")]
    [Tooltip("Número de enemigos que hace falta eliminar en 'Desierto' para pasar")]
    public int enemigosParaPasarDesierto = 6;
    
    [Header("Tiempos")]
    public float tiempoNivel1 = 0;
    public float tiempoNivel2 = 0;
    public float tiempoNivel3 = 0;
    public static LevelManager instance;
    public int levelScore;
    public float levelTimer = 60f;
    private string levelName1 = "Aeropuerto";
    private string levelName2 = "Desierto";

    [Header("Estado del juego")]
    public bool isGameActive = false;
    public GameObject mainMenuPanel;

    [Header("Mensajes")]
    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private static bool hasSessionStarted = false;
    private bool levelEndTriggered = false;
    private PlayerHealth playerHealth;

    // NUEVO: Sistema mejorado de conteo de enemigos
    private int totalEnemiesInLevel = 0;
    private int enemiesDefeated = 0;

    void Awake()
    {
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

    void Start()
    {
        levelScore = 0;
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (messageText != null)
        {
            messageText.text = string.Empty;
            messageText.gameObject.SetActive(false);
        }

        if (!hasSessionStarted)
        {
            // Si hay panel, mostrar menú; si no, arrancar el juego directo
            if (mainMenuPanel != null)
                ShowMainMenu();
            else
                StartGameSession();
        }
        else
        {
            StartGameSession();
        }
    }

    void Update()
    {
        if (!isGameActive) return;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == levelName1)
            HandleLevel1();
        else if (currentScene == levelName2)
            HandleLevel2();

        CheckPlayerDeath();
    }

    private void CheckPlayerDeath()
    {
        if (playerHealth != null && playerHealth.health <= 0f && !levelEndTriggered)
        {
            levelEndTriggered = true;
            ShowMessage("Game Over");
            StartCoroutine(GameOverSequence());
        }
    }

    public void OnEnemyKilled()
    {
        if (!isGameActive || levelEndTriggered) return;

        levelScore++;
        enemiesDefeated++;
        Debug.Log($"Enemigo eliminado! Score: {levelScore}/4 - Derrotados: {enemiesDefeated}/{totalEnemiesInLevel}");
    }

    // NUEVO: Método para contar enemigos al inicio del nivel
  public void InitializeLevelEnemies()
{
    string currentScene = SceneManager.GetActiveScene().name;

    if (currentScene == levelName1)
    {
        totalEnemiesInLevel = 4;
        enemiesDefeated = 0;
        levelScore = 0;
        Debug.Log($"Nivel 1 inicializado: {totalEnemiesInLevel} enemigos objetivo");
    }
    else if (currentScene == levelName2)
    {
        CountEnemiesInScene();

        // Si CountEnemiesInScene no encontró enemigos en el momento de la carga,
        // forzamos el objetivo a 'enemigosParaPasarDesierto' para evitar pasar instantáneamente.
        if (totalEnemiesInLevel <= 0)
            totalEnemiesInLevel = enemigosParaPasarDesierto;
        else
            totalEnemiesInLevel = Mathf.Max(totalEnemiesInLevel, enemigosParaPasarDesierto);

        enemiesDefeated = 0;
        Debug.Log($"Nivel 2 inicializado: {totalEnemiesInLevel} enemigos objetivo (detectados o forzados)");
    }
}


    private void CountEnemiesInScene()
    {
        MutantEnemy[] mutantEnemies = FindObjectsOfType<MutantEnemy>();
        WarrokEnemy[] warrokEnemies = FindObjectsOfType<WarrokEnemy>();
        GameObject[] taggedEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        totalEnemiesInLevel = mutantEnemies.Length + warrokEnemies.Length;

        if (totalEnemiesInLevel == 0 && taggedEnemies.Length > 0)
        {
            totalEnemiesInLevel = taggedEnemies.Length;
        }

        Debug.Log($"Enemigos encontrados: Mutants={mutantEnemies.Length}, Warroks={warrokEnemies.Length}, Tagged={taggedEnemies.Length}, Total={totalEnemiesInLevel}");
    }

    private bool AreAllEnemiesDefeated()
    {
        if (enemiesDefeated >= totalEnemiesInLevel && totalEnemiesInLevel > 0)
        {
            Debug.Log($"Todos los enemigos derrotados por contador: {enemiesDefeated}/{totalEnemiesInLevel}");
            return true;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == levelName1)
        {
            if (levelScore >= 4)
            {
                Debug.Log("Nivel 1 completado: 4/4 enemigos eliminados");
                return true;
            }
        }
        else if (currentScene == levelName2)
        {
            return CheckActiveEnemiesInScene();
        }

        return false;
    }

    private bool CheckActiveEnemiesInScene()
    {
        MutantEnemy[] mutantEnemies = FindObjectsOfType<MutantEnemy>();
        WarrokEnemy[] warrokEnemies = FindObjectsOfType<WarrokEnemy>();

        foreach (var enemy in mutantEnemies)
            if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.EstaMuerto())
                return false;

        foreach (var enemy in warrokEnemies)
            if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.IsDead())
                return false;

        GameObject[] taggedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in taggedEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                MutantEnemy mutant = enemy.GetComponent<MutantEnemy>();
                WarrokEnemy warrok = enemy.GetComponent<WarrokEnemy>();

                if ((mutant != null && !mutant.EstaMuerto()) ||
                    (warrok != null && !warrok.IsDead()) ||
                    (mutant == null && warrok == null))
                    return false;
            }
        }

        Debug.Log("¡Todos los enemigos han sido derrotados!");
        return true;
    }

    public void StartGame()
    {
        hasSessionStarted = true;
        levelEndTriggered = false;
        levelScore = 0;
        enemiesDefeated = 0;
        levelTimer = 60f;

        StartGameSession();
        Debug.Log("Juego iniciado desde Aeropuerto!");
    }

    private void StartGameSession()
    {
        isGameActive = true;

        // ✅ Si hay panel, se oculta. Si no hay, no pasa nada ni se pausa el juego
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerHealth == null)
            playerHealth = FindObjectOfType<PlayerHealth>();

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowMainMenu()
    {
        // ✅ Si no hay panel, no se pausa ni se bloquea
        if (mainMenuPanel == null)
        {
            Debug.LogWarning("⚠ No hay panel de menú principal asignado. Iniciando directamente el juego.");
            StartGameSession();
            return;
        }

        isGameActive = false;
        mainMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

private void HandleLevel1()
{
    if (levelEndTriggered) return;

    if (levelScore < 4)
    {
        if (levelTimer > 0f)
            levelTimer -= Time.deltaTime;
        else if (!levelEndTriggered)
        {
            levelEndTriggered = true;
            ShowMessage("Game Over - Tiempo agotado");
            StartCoroutine(GameOverSequence());
        }
    }
    else if (!levelEndTriggered)
    {
        levelEndTriggered = true;

        // 🔥 Guardar tiempo real del nivel 1
        tiempoNivel1 = Time.timeSinceLevelLoad;

        ShowMessage("Aeropuerto Completado!");
        StartCoroutine(LoadAfterDelay("Desierto", messageDuration));
    }
}

  private void HandleLevel2()
{
    if (levelEndTriggered) return;

    bool levelCompleted = AreAllEnemiesDefeated();

    if (levelCompleted && !levelEndTriggered)
    {
        levelEndTriggered = true;

        // 🔥 Guardar tiempo real del nivel 2
        tiempoNivel2 = Time.timeSinceLevelLoad;

        StartCoroutine(LoadAfterDelay("Scene_B", messageDuration));
    }
}

private IEnumerator GameOverSequence()
{
    ShowMessage("Game Over");

    Time.timeScale = 0f;
    yield return new WaitForSecondsRealtime(messageDuration);

    string currentScene = SceneManager.GetActiveScene().name;

    Time.timeScale = 1f;
    SceneManager.LoadScene(currentScene);

    // Reset
    levelEndTriggered = false;
    levelScore = 0;
    enemiesDefeated = 0;
    levelTimer = 60f;

    yield return new WaitForSecondsRealtime(0.1f);

    StartGameSession(); // 🔥 NO VOLVER AL MENÚ, REINICIAR DIRECTO
}


    private IEnumerator ShowLevelCompletedThenMenu()
    {
        ShowMessage("¡Juego Completado!");
        yield return new WaitForSeconds(messageDuration);
        ShowMainMenu();
    }

    private void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            StartCoroutine(HideMessageAfterDelay());
        }
        Debug.Log(msg);
    }
private IEnumerator LoadAfterDelay(string sceneName, float delay)
{
    yield return new WaitForSeconds(delay);

    string escenaActual = SceneManager.GetActiveScene().name;

    // 🔥 Guardar tiempo del Nivel 1 CUANDO SALES DEL AEROPUERTO
    if (escenaActual == "Aeropuerto")
        tiempoNivel1 = Time.timeSinceLevelLoad;

    // 🔥 Guardar tiempo del Nivel 2 CUANDO SALES DEL DESIERTO
    if (escenaActual == "Desierto")
        tiempoNivel2 = Time.timeSinceLevelLoad;

    SceneManager.LoadScene(sceneName);
}




    private IEnumerator HideMessageAfterDelay()
{
    yield return new WaitForSecondsRealtime(messageDuration);
    if (messageText != null)
        messageText.gameObject.SetActive(false);
}

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // 🔥 Ocultar menú SIEMPRE al cargar cualquier escena (solo aparece al inicio del juego)
    if (mainMenuPanel != null)
        mainMenuPanel.SetActive(false);

    // 🔥 Reiniciar el mensaje de UI
    if (messageText != null)
    {
        messageText.text = "";
        messageText.gameObject.SetActive(false);
    }

    // 🔥 Volver a encontrar al jugador después de recargar escena
    playerHealth = FindObjectOfType<PlayerHealth>();

    // Reset de estado interno del nivel
    levelEndTriggered = false;

    // 🔥 Inicializar enemigos correctamente según la escena
    InitializeLevelEnemies();

    // Configuraciones por escena
    if (scene.name == levelName1) // Aeropuerto
    {
        levelScore = 0;
        enemiesDefeated = 0;
        levelTimer = 60f;
        Debug.Log("Cargando Nivel 1 - Sistema de 4 enemigos");
    }
    else if (scene.name == levelName2) // Desierto
    {
        Debug.Log("Cargando Nivel 2 - Sistema de enemigos jefe");
    }

    // 🔥 Asegurar que el juego esté corriendo después de cargar la escena
    if (isGameActive)
    {
        Time.timeScale = 1f;
    }
}


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
