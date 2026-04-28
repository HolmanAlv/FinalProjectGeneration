using UnityEngine;

public class MainHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public MainHealthUI healthUI;

    private CinematicasManager cinematicasManager;

     void Start()
    {
        cinematicasManager = CinematicasManager.Instance;

        if (cinematicasManager == null)
        {
            Debug.LogError("CinematicasManager no encontrado en la escena");
        }

        if (NewOrLoad.Instance == null || !NewOrLoad.Instance.loadGame)
        {
            currentHealth = maxHealth;
        }

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("Vida global: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("Vida global restaurada: " + currentHealth);
    }

    void GameOver()
    {
        if (cinematicasManager != null)
        {
            cinematicasManager.GameOver = true;
        }
        else
        {
            Debug.LogError("No se puede hacer GameOver porque cinematicasManager es null");
        }
    }

}