using System.Collections;
using UnityEngine;

public class DayNightClockUI : MonoBehaviour
{
    [SerializeField] private RectTransform arrow;

    [Header("Referencias")]
    private DayNiightManager dayNightManager;

    private Coroutine rotateCoroutine;
    private float currentZRotation = 0f;





    private void Start()
    {
        dayNightManager = DayNiightManager.Instance;

        if (dayNightManager == null)
        {
            Debug.LogError("DayNightManager no encontrado");
            return;
        }

        dayNightManager.OnDayStarted += HandleDayStarted;
        dayNightManager.OnNightStarted += HandleNightStarted;

        if (arrow != null)
        {
            currentZRotation = 0f;
            arrow.localRotation = Quaternion.Euler(0f, 0f, currentZRotation);
        }
        else
        {
            Debug.LogError("No asignaste la flecha en el Inspector");
            return;
        }

        Debug.Log("Reloj conectado al DayNightManager");

        // IMPORTANTE: iniciar la rotación al cargar la escena
        HandleDayStarted();
    }

    private void OnDestroy()
    {
        if (dayNightManager != null)
        {
            dayNightManager.OnDayStarted -= HandleDayStarted;
            dayNightManager.OnNightStarted -= HandleNightStarted;
        }
    }

    private void HandleDayStarted()
    {
        float totalDuration = dayNightManager.DayDuration + dayNightManager.TransitionToNightDuration;
        RotateArrow(totalDuration);
    }

    private void HandleNightStarted(int nightNumber)
    {
        float totalDuration = dayNightManager.NightDuration + dayNightManager.TransitionToDayDuration;
        RotateArrow(totalDuration);
    }

    private void RotateArrow(float duration)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        rotateCoroutine = StartCoroutine(RotateArrowRoutine(duration));
    }

    private IEnumerator RotateArrowRoutine(float duration)
    {
        float startRotation = currentZRotation;
        float targetRotation = currentZRotation - 180f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float zRotation = Mathf.Lerp(startRotation, targetRotation, t);
            arrow.localRotation = Quaternion.Euler(0f, 0f, zRotation);

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentZRotation = targetRotation;
        arrow.localRotation = Quaternion.Euler(0f, 0f, currentZRotation);
    }
}
