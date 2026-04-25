using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpcionesUI : MonoBehaviour
{
    [Header("UI")]
    public Slider sliderMusica;
    public TMP_Dropdown dropdownResolucion;

    private Resolution[] resoluciones;

    void Start()
    {
        ConfigurarMusica();
        ConfigurarResoluciones();
    }

    void ConfigurarMusica()
    {
        // El "1f" al final es el valor que usará si NO encuentra nada guardado
        float volumenGuardado = PlayerPrefs.GetFloat("MusicVolume", 1f); 
        sliderMusica.value = volumenGuardado;
    }
    
    void ConfigurarResoluciones()
    {
        dropdownResolucion.ClearOptions();

        List<string> opciones = new List<string>()
        {
            "Pantalla Completa",
            "1920 x 1080",
            "1280 x 720",
            "800 x 600"
        };

        resoluciones = new Resolution[]
        {
            new Resolution() { width = 1920, height = 1080 },
            new Resolution() { width = 1920, height = 1080 },
            new Resolution() { width = 1280, height = 720 },
            new Resolution() { width = 800,  height = 600 }
        };

        dropdownResolucion.AddOptions(opciones);

        int indiceGuardado = PlayerPrefs.GetInt("ResolutionIndex", 0);
        dropdownResolucion.value = indiceGuardado;
        dropdownResolucion.RefreshShownValue();
    }

    public void CambiarMusica(float valor)
    {
        PlayerPrefs.SetFloat("MusicVolume", valor);
        PlayerPrefs.Save();
    }

    public void CambiarResolucion(int indice)
    {
        Resolution res = resoluciones[indice];
        bool esFullscreen = (indice == 0); // Solo la primera es pantalla completa
        Screen.SetResolution(res.width, res.height, esFullscreen);

        PlayerPrefs.SetInt("ResolutionIndex", indice);
        PlayerPrefs.Save();
    }
}