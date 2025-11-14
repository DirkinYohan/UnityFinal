using UnityEngine;

public class Level2Controller : MonoBehaviour
{
    public static Level2Controller Instance;

    private bool completado = false;

    void Awake()
    {
        Instance = this;
    }

    public void CompletarNivel2()
    {
        if (completado) return;
        completado = true;

        LevelManager.instance.tiempoNivel2 = Time.timeSinceLevelLoad;

        Debug.Log("Tiempo del Nivel 2 guardado: " + LevelManager.instance.tiempoNivel2);
    }
}
