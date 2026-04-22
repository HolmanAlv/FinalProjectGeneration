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
    public GameObject textCooldown;

    private bool isAvailable = true;
    private float cooldownTimer;

   private DayNiightManager dayNiightManager;

    private void Awake()
    {
        
    }

    private void Start()
    {

        dayNiightManager = DayNiightManager.Instance;

        if (dayNiightManager == null)
        {
            Debug.LogError("DayNightManager no encontrado en la escena");
        }
        
        textRecolectar.SetActive(false);
        textCooldown.SetActive(false);
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
            Debug.Log("Energía insuficiente");
            return;
        }

        if (resourceType == ResourceType.Wood)
            playerResources.AddWood(resourceAmount);
        else
            playerResources.AddStone(resourceAmount);

        isAvailable = false;
        cooldownTimer = cooldownTime;

        UpdateUI();
    }

    private bool CanInteractByTime()
    {
        return dayNiightManager != null && dayNiightManager.CurrentState == DayNiightManager.DayNightState.Day;
    }
}
