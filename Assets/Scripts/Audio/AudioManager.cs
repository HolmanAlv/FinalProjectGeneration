using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// Este componente centraliza todo el audio del juego.
// Usa patron Singleton para que exista una sola instancia y no se pierda al cambiar de escena.
public class AudioManager : MonoBehaviour
{
    // Acceso global desde cualquier script: AudioManager.Instance
    public static AudioManager Instance;

    [Header("Sources")]
    // Canal dedicado a musica de fondo.
    public AudioSource musicSource;

    // Canal para ambiente general continuo (ej: viento, lluvia).
    public AudioSource ambientSource;

    // Canal para ambiente puntual o segunda capa de ambiente.
    public AudioSource ambientSpotSource;

    // Canal para efectos de juego (SFX) reproducidos con OneShot.
    public AudioSource sfxSource;

    // Canal para sonidos de UI (botones, menus, hover, etc).
    public AudioSource uiSource;

    // Entrada simple para el Inspector.
    // name: clave con la que buscaras el clip desde codigo.
    // clip: archivo de audio real.
    // Si name viene vacio, se usa clip.name como clave automatica.
    [Serializable]
    public struct SoundEntry
    {
        public string name;

        public AudioClip clip;
    }

    [Header("Clips")]
    // Listas configurables desde Inspector por categoria.
    public List<SoundEntry> musicClips;
    public List<SoundEntry> ambientClips;
    public List<SoundEntry> ambientSpotClips;
    public List<SoundEntry> sfxClips;
    public List<SoundEntry> uiClips;

    // Diccionarios internos para busqueda rapida por nombre.
    Dictionary<string, AudioClip> musicDict;
    Dictionary<string, AudioClip> ambientDict;
    Dictionary<string, AudioClip> ambientSpotDict;
    Dictionary<string, AudioClip> sfxDict;
    Dictionary<string, AudioClip> uiDict;

    void Awake()
    {
        // Si ya existe una instancia, esta copia se destruye para evitar duplicados.
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Esta instancia se vuelve la principal y persiste entre escenas.
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Convertimos listas del Inspector a diccionarios para acceso rapido.
        musicDict    = LoadDict(musicClips);
        ambientDict  = LoadDict(ambientClips);
        ambientSpotDict = LoadDict(ambientSpotClips);
        sfxDict      = LoadDict(sfxClips);
        uiDict       = LoadDict(uiClips);
    }

    void Start()
    {
        // Espacio reservado por si luego quieres lanzar musica inicial automaticamente.
        //PlayMusic(AudioManager.Instance.musicClips);
    }

    Dictionary<string, AudioClip> LoadDict(List<SoundEntry> entries)
    {
        // Crea un diccionario con la clave definida por name o por clip.name si name esta vacio.
        var dict = new Dictionary<string, AudioClip>();
        foreach (var entry in entries)
            if (entry.clip != null && !string.IsNullOrEmpty(entry.name))
            { 
                dict[entry.name] = entry.clip;
            }
            else if (entry.clip != null && string.IsNullOrEmpty(entry.name))
            {
                dict[entry.clip.name] = entry.clip;
            }

        return dict;
    }

    // -------------------------------------------------------
    //                        MUSIC
    // -------------------------------------------------------
    // Reproduce una pista de musica por nombre con transicion suave.
    // fadeTime define cuanto tarda el cambio entre canciones.
    public void PlayMusic(string name, float fadeTime = 1f)
    {
        if (!musicDict.ContainsKey(name)) return;
        StartCoroutine(FadeInMusic(musicDict[name], fadeTime));
    }

    // Detiene la musica con fade out.
    public void StopMusic(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutMusic(fadeTime));
    }

    // Si hay musica sonando, primero la apaga suavemente.
    // Luego carga el nuevo clip y sube volumen de 0 al volumen objetivo.
    IEnumerator FadeInMusic(AudioClip newClip, float time)
    {
        float targetVol = musicSource.volume > 0 ? musicSource.volume : 1f;

        if (musicSource.isPlaying)
            yield return StartCoroutine(FadeOutMusic(time));

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        float t = 0;
        while (t < time)
        {
            musicSource.volume = Mathf.Lerp(0, targetVol, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = targetVol;
    }

    // Baja volumen de forma gradual hasta 0 y luego detiene el canal de musica.
    IEnumerator FadeOutMusic(float time)
    {
        float startVol = musicSource.volume;
        float t = 0;
        while (t < time)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = 0;
        musicSource.Stop();
    }

    // -------------------------------------------------------
    //                      CATEGORY SFX
    // -------------------------------------------------------
    // Reproduce un SFX puntual por nombre.
    public void PlaySFX(string name)
        => PlayFromDict(sfxDict, sfxSource, name);

    // Reproduce sonido de interfaz por nombre.
    public void PlayUI(string name)
        => PlayFromDict(uiDict, uiSource, name);

// -------------------------------------------------------
    //                      CATEGORY Ambience
    // -------------------------------------------------------
    // Reproduce un ambiente en loop en el canal principal de ambiente.
    public void PlayAmbience(string name)
    {
        if (!ambientDict.ContainsKey(name)) return;
        ambientSource.clip = ambientDict[name];
        ambientSource.loop = true;
        ambientSource.Play();
    }
    
    // Reproduce un ambiente secundario en loop (ejemplo: fogata, maquina, etc).
    public void PlayAmbientSpot(string name)
    {
        if (!ambientSpotDict.ContainsKey(name)) return;
        ambientSpotSource.clip = ambientSpotDict[name];
        ambientSpotSource.loop = true;
        ambientSpotSource.Play();
    }

    // Detiene de inmediato el ambiente principal.
    public void StopAmbience() => ambientSource.Stop();
    public void StopAmbientSpot() => ambientSpotSource.Stop();
    

    // Detiene de inmediato el canal de UI.
    public void StopUI() => uiSource.Stop();

    // Detiene todos los canales de audio y cancela fades/corrutinas activas.
    public void StopAllSounds()
    {
        StopAllCoroutines();

        if (musicSource != null) musicSource.Stop();
        if (ambientSource != null) ambientSource.Stop();
        if (ambientSpotSource != null) ambientSpotSource.Stop();
        if (sfxSource != null) sfxSource.Stop();
        if (uiSource != null) uiSource.Stop();
    }

    // Metodo auxiliar: busca un clip por nombre y lo reproduce en OneShot.
    // OneShot permite reproducir sin cortar clips que ya suenan en ese source.
    void PlayFromDict(Dictionary<string, AudioClip> dict, AudioSource source, string name)
    {
        if (dict.ContainsKey(name))
            source.PlayOneShot(dict[name]);
    }

    // Metodo pensado para sliders de UI.
    // Limita el volumen al rango valido de Unity: 0 a 1.
    public void ChangeVolumeWithSlider(AudioSource audioSourceToChange, float newVolume) {

        newVolume = newVolume > 1f ? 1 : newVolume < 0f ? 0 : newVolume;
        audioSourceToChange.volume = newVolume;
    }
    
}