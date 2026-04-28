using UnityEngine;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class MenuPausa : MonoBehaviour
{
    public GameObject panelPausa;
    public OpcionesUI scriptOpciones; // Arrastra aquí el panel de opciones que ya tenemos

    private bool juegoPausado = false;

    void Update()
    {
        // También puedes pausar con la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        juegoPausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f; // CONGELA EL TIEMPO DEL JUEGO
    }

    public void Reanudar()
    {
        juegoPausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f; // REGRESA EL TIEMPO A LA NORMALIDAD
    }

    public void AbrirOpciones()
    {
        // Usamos el método que creamos antes para que el panel sepa que viene de pausa
        scriptOpciones.AbrirDesdePausa();
        panelPausa.SetActive(false); // Escondemos el de pausa mientras tanto
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Asegúrate que el nombre sea exacto
    }
}