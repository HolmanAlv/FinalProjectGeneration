using UnityEngine;
using TMPro;
using System;

public class InteractableResources : MonoBehaviour
{
    
    public enum ResourceType { Wood, Stone }
    public ResourceType resourceType;

    public int energyCost = 1;
    public int resourceAmount = 1;
    public float cooldownTime = 20f;

    public GameObject textRecolectar;
    public GameObject insufficientMat;
    public GameObject textCooldown;

    private bool showingInsufficient = false;

    private bool isAvailable = true;
    private float cooldownTimer;

    [Header("Modelos visuales")]
    public GameObject modeloDisponible;
    public GameObject modeloUsado;

   private DayNiightManager dayNiightManager; // cuando es una instancia le quitamos el serializefield.


    private void Start()
    {

        dayNiightManager = DayNiightManager.Instance;

        if (dayNiightManager == null)
        {
            Debug.LogError("DayNightManager no encontrado en la escena");
        }

        textRecolectar.SetActive(false);
        textCooldown.SetActive(false);
        insufficientMat.SetActive(false);

        UpdateModel();
    }

    private void Update()
    {
        if (!isAvailable)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isAvailable = true;
                cooldownTimer = 0f;
                UpdateModel();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        UpdateUI();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        UpdateUI();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        textRecolectar.SetActive(false);
        textCooldown.SetActive(false);
    }

    void UpdateUI()
    {
         if (showingInsufficient)
        {
            textRecolectar.SetActive(false);
            textCooldown.SetActive(false);
            return;
        }
        

        if (!CanInteractByTime())
        {
            textRecolectar.SetActive(false);
            textCooldown.SetActive(false);
            return;
        }

        if (isAvailable)
        {
            textRecolectar.SetActive(true);
            textCooldown.SetActive(false);
        }
        else
        {
            textRecolectar.SetActive(false);
            textCooldown.SetActive(true);
        }
    }

    public void Interact(GameObject player)
    {
        if (!isAvailable)
            return;
    
        if (!CanInteractByTime())
        {     
            Debug.Log("Este recurso solo puede recolectarse de día.");
            return;
        }


        PlayerEnergy playerEnergy = player.GetComponent<PlayerEnergy>();
        PlayerResources playerResources = player.GetComponent<PlayerResources>();

        if (playerEnergy == null || playerResources == null)
        {
            Debug.LogWarning("Falta PlayerEnergy o PlayerResources en el jugador.");
            return;
        }

        if (!playerEnergy.ConsumeEnergy(energyCost))
        {
            showingInsufficient = true;

            Debug.Log("Energía insuficiente");

            insufficientMat.SetActive(true);
            textRecolectar.SetActive(false);
            textCooldown.SetActive(false);

            CancelInvoke(nameof(ResetUI));
            Invoke(nameof(ResetUI), 2f);

            return;
        }

        if (resourceType == ResourceType.Wood)
            playerResources.AddWood(resourceAmount);
        else
            playerResources.AddStone(resourceAmount);

        isAvailable = false;
        cooldownTimer = cooldownTime;

        UpdateModel();
        UpdateUI();
    }

    void ResetUI()
    {
        showingInsufficient = false;
        insufficientMat.SetActive(false);
        UpdateUI();
    }

    

    private void UpdateModel()
    {
        if (modeloDisponible == null || modeloUsado == null)
            return;

        modeloDisponible.SetActive(isAvailable);
        modeloUsado.SetActive(!isAvailable);
    }

    private bool CanInteractByTime()
    {
        return dayNiightManager != null && dayNiightManager.CurrentState == DayNiightManager.DayNightState.Day;
    }
}
