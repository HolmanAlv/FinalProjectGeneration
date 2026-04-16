using UnityEngine;
using UnityEngine.UI;

public class StructureHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public Image fillImage;
    public Canvas canvas;

    [Header("Vida")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Rotación de la Barra")]
    public Vector3 customRotation = new Vector3(0, 0, 0);
    public bool usarRotacionPersonalizada = true;

    void LateUpdate()
    {
        if (canvas != null)
        {
            if (usarRotacionPersonalizada)
            {
                // Rotación personalizada ajustable desde el inspector
                canvas.transform.rotation = Quaternion.Euler(customRotation);
            }
            else
            {
                // Rotación fija por defecto
                canvas.transform.rotation = Quaternion.identity;
            }
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;

        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);

        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }
    }
}