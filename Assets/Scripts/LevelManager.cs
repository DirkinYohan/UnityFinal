using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
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

        // ✅ CORRECCIÓN: Siempre mostrar el menú principal al inicio
        // a menos que la sesión ya esté activa
        if (!hasSessionStarted)
        {
            ShowMainMenu();
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
        Debug.Log($"Enemigo eliminado! Score: {levelScore}/4");
    }

    public void StartGame()
    {
        hasSessionStarted = true;
        levelEndTriggered = false;
        levelScore = 0;
        levelTimer = 60f;
        
        StartGameSession();
        Debug.Log("Juego iniciado desde Aeropuerto!");
    }

    private void StartGameSession()
    {
        isGameActive = true;
        
        // ✅ CORRECCIÓN: Ocultar el panel del menú principal
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
        isGameActive = false;
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleLevel1()
    {
        if(levelEndTriggered) return;

        if(levelScore < 4)
        {
            if(levelTimer > 0f)
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
            ShowMessage("Aeropuerto Completado!");
            StartCoroutine(LoadAfterDelay(levelName2, messageDuration));
        }
    }

    private void HandleLevel2()
    {
        if(levelEndTriggered) return;

        var enemy = FindObjectOfType<MutantEnemy>();
        if(enemy == null && !levelEndTriggered)
        {
            levelEndTriggered = true;
            StartCoroutine(ShowLevelCompletedThenMenu());
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
        
        levelEndTriggered = false;
        levelScore = 0;
        levelTimer = 60f;
        
        yield return new WaitForSeconds(0.1f);
        ShowMainMenu();
    }

    private IEnumerator ShowLevelCompletedThenMenu()
    {
        ShowMessage("¡Juego Completado!");
        yield return new WaitForSeconds(messageDuration);
        ShowMainMenu();
    }

    private void ShowMessage(string msg)
    {
        if(messageText != null)
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
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if(messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        levelEndTriggered = false;
        
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