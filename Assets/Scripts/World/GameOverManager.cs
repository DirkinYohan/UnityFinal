using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance { get; private set; }

    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = "GAME OVER";

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
