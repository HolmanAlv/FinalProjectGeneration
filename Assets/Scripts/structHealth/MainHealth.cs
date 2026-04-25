using UnityEngine;

public class MainHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public MainHealthUI healthUI;

    public CinematicasManager cinematicasManager;

    void Start()
    {
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
        cinematicasManager.GameOver = true;
    }
}