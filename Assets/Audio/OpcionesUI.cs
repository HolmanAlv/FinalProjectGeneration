using UnityEngine;
using UnityEngine.UI;

public class OpcionesUI : MonoBehaviour
{
    [Header("UI - Ajustes")]
    public Slider sliderMusica;
    public Toggle togglePantallaCompleta;

    [Header("UI - Navegación")]
    public GameObject contenedorMenuPrincipal;
    public GameObject contenedorMenuPausa;

    private bool vieneDePausa = false;

        void Start()
    {
        ConfigurarPantalla(); // Primero pantalla (para que el Toggle se conecte)
        ConfigurarMusica();   // Después música (aunque falle, no afecta el Toggle)
    }

    // --- MÚSICA ---
    void ConfigurarMusica()
    {
        float volumenGuardado = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sliderMusica.value = volumenGuardado;
    }

    public void CambiarMusica(float valor)
    {
        PlayerPrefs.SetFloat("MusicVolume", valor);
        PlayerPrefs.Save();
    }

    // --- PANTALLA ---
    void ConfigurarPantalla()
    {
        bool esFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        togglePantallaCompleta.onValueChanged.RemoveListener(CambiarPantalla);
        togglePantallaCompleta.isOn = esFullscreen;
        togglePantallaCompleta.onValueChanged.AddListener(CambiarPantalla);

        Screen.SetResolution(1920, 1080, esFullscreen);
    }

    public void CambiarPantalla(bool esFullscreen)
    {
        Debug.Log("Toggle recibió: " + esFullscreen); // ← ¿Qué valor llega?
        Screen.SetResolution(1920, 1080, esFullscreen);
        PlayerPrefs.SetInt("Fullscreen", esFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log(esFullscreen ? "🖥️ Pantalla Completa" : "🪟 Modo Ventana");
    }

    // --- NAVEGACIÓN ---
    public void AbrirDesdeMenu()
    {
        vieneDePausa = false;
        if (contenedorMenuPrincipal != null) contenedorMenuPrincipal.SetActive(false);
        gameObject.SetActive(true);
    }

    public void AbrirDesdePausa()
    {
        vieneDePausa = true;
        if (contenedorMenuPausa != null) contenedorMenuPausa.SetActive(false);
        gameObject.SetActive(true);
    }

    public void BotonVolver()
    {
        gameObject.SetActive(false);

        if (vieneDePausa)
        {
            if (contenedorMenuPausa != null) contenedorMenuPausa.SetActive(true);
            Debug.Log("⬅️ Volviendo a Pausa");
        }
        else
        {
            if (contenedorMenuPrincipal != null) contenedorMenuPrincipal.SetActive(true);
            Debug.Log("⬅️ Volviendo al Menú Principal");
        }
    }
}