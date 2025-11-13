using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Panel Objetivos (tecla 1)")]
    public GameObject objetivosPanel;

    [Header("Panel Controles (tecla 2)")]
    public GameObject controlesPanel;

    private bool objetivosOpen = false;
    private bool controlesOpen = false;

    void Update()
    {
        // Tecla 1 → Mostrar/Ocultar Objetivos
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            objetivosOpen = !objetivosOpen;
            objetivosPanel.SetActive(objetivosOpen);
        }

        // Tecla 2 → Mostrar/Ocultar Controles
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlesOpen = !controlesOpen;
            controlesPanel.SetActive(controlesOpen);
        }
    }
}
