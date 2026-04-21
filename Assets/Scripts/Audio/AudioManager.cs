using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource ambientSpotSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Serializable]
    public struct SoundEntry
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Clips")]
    public List<SoundEntry> musicClips;
    public List<SoundEntry> ambientClips;
    public List<SoundEntry> ambientSpotClips;
    public List<SoundEntry> sfxClips;
    public List<SoundEntry> uiClips;

    Dictionary<string, AudioClip> musicDict;
    Dictionary<string, AudioClip> ambientDict;
    Dictionary<string, AudioClip> ambientSpotDict;

    Dictionary<string, AudioClip> sfxDict;
    Dictionary<string, AudioClip> uiDict;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicDict    = LoadDict(musicClips);
        ambientDict  = LoadDict(ambientClips);
        ambientSpotDict = LoadDict(ambientSpotClips);
        sfxDict      = LoadDict(sfxClips);
        uiDict       = LoadDict(uiClips);
    }

    void Start()
    {
        PlayMusic("endy");
    }

    Dictionary<string, AudioClip> LoadDict(List<SoundEntry> entries)
    {
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
    public void PlayMusic(string name, float fadeTime = 1f)
    {
        if (!musicDict.ContainsKey(name)) return;
        StartCoroutine(FadeInMusic(musicDict[name], fadeTime));
    }

    public void StopMusic(float fadeTime = 1f)
    {
        StartCoroutine(FadeOutMusic(fadeTime));
    }

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
    public void PlaySFX(string name)
        => PlayFromDict(sfxDict, sfxSource, name);

    public void PlayUI(string name)
        => PlayFromDict(uiDict, uiSource, name);
// -------------------------------------------------------
    //                      CATEGORY Ambience
    // -------------------------------------------------------
    public void PlayAmbience(string name)
    {
        if (!ambientDict.ContainsKey(name)) return;
        ambientSource.clip = ambientDict[name];
        ambientSource.loop = true;
        ambientSource.Play();
    }
    
    public void PlayAmbientSpot(string name)
    {
        if (!ambientSpotDict.ContainsKey(name)) return;
        ambientSpotSource.clip = ambientSpotDict[name];
        ambientSpotSource.loop = true;
        ambientSpotSource.Play();
    }

    public void StopAmbience() => ambientSource.Stop();
    public void StopUI() => uiSource.Stop();

    void PlayFromDict(Dictionary<string, AudioClip> dict, AudioSource source, string name)
    {
        if (dict.ContainsKey(name))
            source.PlayOneShot(dict[name]);
    }
    public void ChangeVolumeWithSlider(AudioSource audioSourceToChange, float newVolume) {

        newVolume = newVolume > 1f ? 1 : newVolume < 0f ? 0 : newVolume;
        audioSourceToChange.volume = newVolume;
    }
    
}