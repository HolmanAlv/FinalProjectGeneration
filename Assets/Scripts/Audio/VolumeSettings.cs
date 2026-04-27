using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider generalSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        if(PlayerPrefs.HasKey("masterVolume") && PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadVolumes();
        }
        else
        {
            // Si no hay valores guardados, establecer los sliders al valor predeterminado (1.0f)
            generalSlider.value = 1.0f;
            musicSlider.value = 1.0f;
            sfxSlider.value = 1.0f;

            SetGeneralVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }
    public void SetGeneralVolume()
    {
        float volumen = generalSlider.value;
        audioMixer.SetFloat("master", Mathf.Log10(volumen)*20);
        PlayerPrefs.SetFloat("masterVolume", volumen);
    }

    public void SetMusicVolume()
    {
        float volumen = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volumen)*20);
        PlayerPrefs.SetFloat("musicVolume", volumen);
    }

    public void SetSFXVolume()
    {
        float volumen = sfxSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volumen)*20);
        PlayerPrefs.SetFloat("sfxVolume", volumen);
    }

    private void LoadVolumes()
    {
        generalSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        SetGeneralVolume();
        SetMusicVolume();
        SetSFXVolume();
    }
}
