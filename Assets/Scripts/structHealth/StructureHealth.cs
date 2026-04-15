using UnityEngine;

public class StructureHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public MainHealth mainHealth; // referencia a la vida global

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

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