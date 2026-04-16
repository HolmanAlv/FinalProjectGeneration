using UnityEngine;

public class StructureHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public MainHealth mainHealth; // referencia a la vida global

    public StructureHealthUI healthUI; // referencia al UI de salud

    void Start()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.SetTarget(transform);
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " vida: " + currentHealth);
        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth, maxHealth);
        }
        // Reducir también la vida global
        if (mainHealth != null)
        {
            mainHealth.TakeDamage(damage / 4f);
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

}