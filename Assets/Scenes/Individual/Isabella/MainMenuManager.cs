using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void Start()
    {
        AudioManager.Instance.PlayMusic("main_menu");
    }
    // Función para "Nueva partida"
    public void NuevaPartida()
    {
        clickSound();
        Debug.Log("Iniciando una partida desde cero...");
        // Aquí luego pondremos: SceneManager.LoadScene("NombreEscena");
    }

    // Función para "Continuar"
    public void ContinuarPartida()
    {
        clickSound();
        Debug.Log("Cargando la última partida guardada...");
        // Aquí irá la lógica para cargar datos
    }

    // Función para "Opciones"
    public void AbrirOpciones()
    {
        clickSound();
        Debug.Log("Abriendo el menú de configuraciones...");
        // Aquí luego el panel de opciones
    }

    // Función para "Créditos"
    public void AbrirCreditos()
    {
        clickSound();
        Debug.Log("Mostrando los créditos del equipo...");
        // Aquí luego el panel de créditos
    }

    private void clickSound()
    {
        AudioManager.Instance.PlayUI("click_02");
    }
}