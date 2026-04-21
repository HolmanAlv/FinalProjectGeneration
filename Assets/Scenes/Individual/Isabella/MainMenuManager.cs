using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Función para "Nueva partida"
    public void NuevaPartida()
    {
        Debug.Log("Iniciando una partida desde cero...");
        // Aquí luego pondremos: SceneManager.LoadScene("NombreEscena");
    }

    // Función para "Continuar"
    public void ContinuarPartida()
    {
        Debug.Log("Cargando la última partida guardada...");
        // Aquí irá la lógica para cargar datos
    }

    // Función para "Opciones"
    public void AbrirOpciones()
    {
        Debug.Log("Abriendo el menú de configuraciones...");
        // Aquí luego el panel de opciones
    }

    // Función para "Créditos"
    public void AbrirCreditos()
    {
        Debug.Log("Mostrando los créditos del equipo...");
        // Aquí luego el panel de créditos
    }
}