using UnityEngine;

public class MainHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public MainHealthUI healthUI;

    void Start()
    {
        currentHealth = maxHealth;
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

    void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}