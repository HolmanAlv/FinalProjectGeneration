using UnityEngine;

public class StructureHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public MainHealth mainHealth; // referencia a la vida global
    public StructureHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Reducir también la vida global
        if (mainHealth != null)
        {
            mainHealth.TakeDamage(damage / 4f);
        }

        // Actualizar barra visual
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            DestroyStructure();
        }
    }

    void DestroyStructure()
    {
        Debug.Log(gameObject.name + " destruida");
        gameObject.SetActive(false);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

}