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
    [SerializeField] private float transitionToDayDuration = 3f;

    [Header("Referencias principales")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Button startNightButton;

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

    [SerializeField] private int currentNightNumber = 0;
    [SerializeField] private int completedNights = 0;

    public int CurrentNightNumber => currentNightNumber;// mirar si esto no se puede hacer con set y get
    public int CompletedNights => completedNights;   

    private Coroutine cycleCoroutine;

    private void Awake()
    {
        if (startNightButton != null)
        {
            startNightButton.onClick.AddListener(StartNightCycle);
        }
    }

    private void Start()
    {
        ApplyDayInstant();
        SetState(DayNightState.Day);
    }

    private void OnDestroy()
    {
        if (startNightButton != null)
        {
            startNightButton.onClick.RemoveListener(StartNightCycle);
        }
    }

    public void StartNightCycle()
    {
        

        if (currentState != DayNightState.Day)
            return;
            
        currentNightNumber ++;

        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
        }

        cycleCoroutine = StartCoroutine(DayNightCycleRoutine());
    }

    private IEnumerator DayNightCycleRoutine()
    {
        SetState(DayNightState.TransitionToNight);
        OnTransitionToNightStarted?.Invoke();

        //Esto ayuda a hacer un interpolado suave y permite una transición gradual
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

        SetState(DayNightState.Night);
        OnNightStarted?.Invoke(currentNightNumber);
        Debug.Log("🌙 Iniciando noche: " + currentNightNumber);


        yield return new WaitForSeconds(nightDuration);

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

        ApplyDayInstant();
        SetState(DayNightState.Day);
        completedNights = currentNightNumber;
        
        OnDayStarted?.Invoke();
        Debug.Log("🌙 noxhes completadas " + completedNights);

        cycleCoroutine = null;
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
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (startNightButton == null)
            return;

        bool canStartNight = currentState == DayNightState.Day;
        startNightButton.interactable = canStartNight;
    }

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