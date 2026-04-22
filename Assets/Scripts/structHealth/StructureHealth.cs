using UnityEngine;

public class StructureHealth : MonoBehaviour
{
    public bool isDestoyed = false;
    public float maxHealth = 100f;
    public float currentHealth;

    public MainHealth mainHealth; // referencia a la vida global
    public StructureHealthUI healthUI; // referencia al UI de salud

    [Header("Reparación")]
    public int woodRequired = 1;
    public int stoneRequired = 0;
    public float repairAmount = 10f;

    [Header("Texto en escena")]
    public GameObject repairTextObject;

    private bool playerInRange = false;
    private PlayerResources currentPlayerResources;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool PlayerInRange => playerInRange;

    private DayNiightManager dayNiightManager;

    void Start()
    {
        dayNiightManager = DayNiightManager.Instance;

        if (dayNiightManager == null)
        {
            Debug.LogError("DayNightManager no encontrado en la escena");
        }
        if (NewOrLoad.Instance != null && !NewOrLoad.Instance.loadGame)
        {
           currentHealth = maxHealth;
        }

        if (healthUI != null)
        {
            healthUI.SetTarget(transform);
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        if (repairTextObject != null)
        {
            repairTextObject.SetActive(false);
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log(gameObject.name + " vida: " + currentHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        if (mainHealth != null)
        {
            mainHealth.TakeDamage(damage / 4f);
        }

        if (currentHealth <= 0 && !isDestoyed)
        {
            DestroyStructure();
        }
    }

    public void TryRepair()
    {
        if (!CanInteractByTime())
        {
            repairTextObject.SetActive(false);
            return;
        }


        if (!playerInRange)
            return;

        if (currentHealth >= maxHealth)
        {
            Debug.Log("La estructura ya tiene la vida completa");
            return;
        }

        if (currentPlayerResources == null)
        {
            Debug.LogWarning("No se encontró PlayerResources en el jugador");
            return;
        }


        if (!currentPlayerResources.ConsumeResources(woodRequired, stoneRequired))
        {
            Debug.Log("Materiales insuficientes");
            return;
        }

        float previousHealth = currentHealth;

        currentHealth += repairAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        float restoredAmount = currentHealth - previousHealth;

        if (mainHealth != null && restoredAmount > 0f)
        {
            mainHealth.RestoreHealth(restoredAmount / 4f);
        }

        Debug.Log(gameObject.name + " reparada. Vida actual: " + currentHealth);

    }


    void DestroyStructure()
    {
        Debug.Log(gameObject.name + " destruida");

        if (repairTextObject != null)
        {
            repairTextObject.SetActive(false);
        }

        gameObject.SetActive(false);
        isDestoyed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        

        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        currentPlayerResources = other.GetComponent<PlayerResources>();

        if (currentPlayerResources == null)
        {
            Debug.LogWarning("El jugador no tiene PlayerResources");
        }

        if (!CanInteractByTime())
        {
            repairTextObject.SetActive(false);
            return;
        }

        // Mostrar el texto SOLO si la estructura no está completa
        if (repairTextObject != null && currentHealth < maxHealth)
        {
            repairTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;
        currentPlayerResources = null;

        if (repairTextObject != null)
        {
            repairTextObject.SetActive(false);
        }
    }

        private bool CanInteractByTime()
    {
        return dayNiightManager != null && dayNiightManager.CurrentState == DayNiightManager.DayNightState.Day;
    }

}