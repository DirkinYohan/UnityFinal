using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCounter : MonoBehaviour
{
    public int totalEnemigos;
    private int enemigosRestantes;

    void Start()
    {
        enemigosRestantes = totalEnemigos;
    }

    public void EnemigoEliminado()
    {
        enemigosRestantes--;

        if (enemigosRestantes <= 0)
        {
            Debug.Log("Todos los enemigos eliminados. Pasando al Nivel 3");

            Level2Controller.Instance.CompletarNivel2();

            SceneManager.LoadScene("Scene_B"); // Nivel 3
        }
    }
}
