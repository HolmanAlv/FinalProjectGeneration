using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayNiightManager : MonoBehaviour
{
    public enum DayNightState
    {
        //definimos maquina de estados
        Day,
        TransitionToNight,
        Night,
        TransitionToDay
    }

    [Header("Estado actual")]
    [SerializeField] private DayNightState currentState = DayNightState.Day;

    [Header("Tiempos")]
    [SerializeField] private float transitionToNightDuration = 3f;
    public float nightDuration = 5.0f;
    public float dayDuration = 20f;
    [SerializeField] private float transitionToDayDuration = 3f;

    [Header("Referencias principales")]
    [SerializeField] private Light directionalLight;
    //[SerializeField] private Button startNightButton;

    [Header("Luz de día")]
    [SerializeField] private float dayLightIntensity = 1.2f;
    public Color dayLightColor = new Color(1f, 0.95f, 0.84f);
    public Color dayAmbientColor = new Color(0.75f, 0.75f, 0.75f);

    [Header("Luz de noche")]
    [SerializeField] private float nightLightIntensity = 0.2f;
    [SerializeField] private Color nightLightColor = new Color(0.35f, 0.45f, 0.7f);
    [SerializeField] private Color nightAmbientColor = new Color(0.18f, 0.2f, 0.28f);

    [Header("Luces secundarias de noche")]
    [SerializeField] private Light[] nightLights;

    public DayNightState CurrentState => currentState;
    public bool IsNight => currentState == DayNightState.Night;
    public bool IsTransitioning => currentState == DayNightState.TransitionToNight ||currentState == DayNightState.TransitionToDay;

    public event Action OnDayStarted;
    public event Action OnTransitionToNightStarted;
    public event Action<int> OnNightStarted;
    public event Action OnTransitionToDayStarted;

    public int currentNightNumber = 0;
    public int completedNights = 0;


    // variable publicas pero no para edición
    public int CurrentNightNumber => currentNightNumber;
    public int CompletedNights => completedNights;   
    public float DayDuration => dayDuration;
    public float TransitionToNightDuration => transitionToNightDuration;
    public float NightDuration => nightDuration;
    public float TransitionToDayDuration => transitionToDayDuration;

    private Coroutine cycleCoroutine;

    public static DayNiightManager Instance;
    //public ManagerData managerData;

    public UIManager uIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        /*if (startNightButton != null)
        {
            startNightButton.onClick.AddListener(StartNightCycle);
        }*/


    }

    private void Start()
    {
        ApplyDayInstant();
        SetState(DayNightState.Day);

        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
        }

        cycleCoroutine = StartCoroutine(DayNightCycleRoutine());
    }

    

    private void OnDestroy()
    {
        /*if (startNightButton != null)
        {
            startNightButton.onClick.RemoveListener(StartNightCycle);
        }*/
    }
    private void PlayNightAudio()
    {
        if (AudioManager.Instance == null)
            return;

        if (AudioManager.Instance.musicSource.isPlaying)
        {
            AudioManager.Instance.StopMusic(0.5f);
            AudioManager.Instance.StopAmbientSpot();
            AudioManager.Instance.StopAmbience();
        }

        AudioManager.Instance.PlayMusic("night_01");
        AudioManager.Instance.PlayAmbientSpot("heartbeat");
        AudioManager.Instance.PlayAmbience("wind_night");
    }

    public void StartNightCycle()
    {
        Debug.LogWarning("StartNightCycle ya no se usa. El ciclo día/noche ahora es automático.");

    }

    private IEnumerator DayNightCycleRoutine()
    {
        while (true)
        {
            // DÍA
            ApplyDayInstant();
            SetState(DayNightState.Day);
            OnDayStarted?.Invoke();

            Debug.Log("☀️ Día iniciado");

            yield return new WaitForSeconds(dayDuration);

            // TRANSICIÓN A NOCHE
            currentNightNumber++;

            PlayNightAudio();

            SetState(DayNightState.TransitionToNight);
            OnTransitionToNightStarted?.Invoke();

            yield return StartCoroutine(TransitionLighting(
                transitionToNightDuration,
                dayLightIntensity,
                nightLightIntensity,
                dayLightColor,
                nightLightColor,
                dayAmbientColor,
                nightAmbientColor,
                false,
                true
            ));

            // NOCHE
            SetState(DayNightState.Night);
            OnNightStarted?.Invoke(currentNightNumber);

            Debug.Log("🌙 Iniciando noche: " + currentNightNumber);

            yield return new WaitForSeconds(nightDuration);

            // TRANSICIÓN A DÍA
            SetState(DayNightState.TransitionToDay);
            OnTransitionToDayStarted?.Invoke();

            yield return StartCoroutine(TransitionLighting(
                transitionToDayDuration,
                nightLightIntensity,
                dayLightIntensity,
                nightLightColor,
                dayLightColor,
                nightAmbientColor,
                dayAmbientColor,
                true,
                false
            ));

            // FIN DE NOCHE
            completedNights = currentNightNumber;

            if (uIManager != null)
            {
                uIManager.UpdateNigth(completedNights);
                //managerData.SaveDataGame();
            }

            Debug.Log("☀️ Noches completadas: " + completedNights);
        }

    }

    private IEnumerator TransitionLighting(
        float duration,
        float startIntensity,
        float targetIntensity,
        Color startLightColor,
        Color targetLightColor,
        Color startAmbientColor,
        Color targetAmbientColor,
        bool turnOffNightLightsAtEnd,
        bool turnOnNightLightsAtEnd
    )
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            if (directionalLight != null)
            {
                directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                directionalLight.color = Color.Lerp(startLightColor, targetLightColor, t);
            }

            RenderSettings.ambientLight = Color.Lerp(startAmbientColor, targetAmbientColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (directionalLight != null)
        {
            directionalLight.intensity = targetIntensity;
            directionalLight.color = targetLightColor;
        }

        RenderSettings.ambientLight = targetAmbientColor;

        if (turnOnNightLightsAtEnd)
        {
            SetNightLights(true);
        }

        if (turnOffNightLightsAtEnd)
        {
            SetNightLights(false);
        }
    }

    private void ApplyDayInstant()
    {
        // Audio del dia
        if (AudioManager.Instance.musicSource.isPlaying)
        {
            AudioManager.Instance.StopMusic(1f);
            AudioManager.Instance.StopAmbience();
            AudioManager.Instance.StopAmbientSpot();
        }
        AudioManager.Instance.PlayMusic("day_01");
        AudioManager.Instance.PlayAmbientSpot("river");
        AudioManager.Instance.PlayAmbience("birds");
        AudioManager.Instance.PlaySFX("day_start");

        if (directionalLight != null)
        {
            directionalLight.intensity = dayLightIntensity;
            directionalLight.color = dayLightColor;
        }

        RenderSettings.ambientLight = dayAmbientColor;
        SetNightLights(false);
    }

    private void ApplyNightInstant()
    {
        if (directionalLight != null)
        {
            directionalLight.intensity = nightLightIntensity;
            directionalLight.color = nightLightColor;
        }

        RenderSettings.ambientLight = nightAmbientColor;
        SetNightLights(true);
    }

    // si hay luces secundarias como lamparas o parecido las enciende durante la noche o apaga durante el dia
    private void SetNightLights(bool active)
    {
        if (nightLights == null) return;

        foreach (Light lightItem in nightLights)
        {
            if (lightItem != null)
            {
                lightItem.enabled = active;
            }
        }
    }

    private void SetState(DayNightState newState)
    {
        currentState = newState;
        //UpdateButtonState();
    }

    /*private void UpdateButtonState()
    {
        if (startNightButton == null)
            return;

        bool canStartNight = currentState == DayNightState.Day;
        startNightButton.interactable = canStartNight;
    }*/

#if UNITY_EDITOR
    [ContextMenu("Forzar Día")]
    private void ForceDay()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
        }

        ApplyDayInstant();
        SetState(DayNightState.Day);
        OnDayStarted?.Invoke();
    }

    [ContextMenu("Forzar Noche")]
    private void ForceNight()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
        }

        ApplyNightInstant();
        SetState(DayNightState.Night);
        OnNightStarted?.Invoke(currentNightNumber);
    }
#endif
}