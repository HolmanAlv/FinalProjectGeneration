using UnityEngine;
using UnityEngine.UI;

public class StructureHealthUI : MonoBehaviour
{
    public Image healthFill; // la imagen verde
    public Vector3 offset = new Vector3(0, 5, 0);
    private Transform target;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        float value = currentHealth / maxHealth;
        healthFill.fillAmount = value;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.forward = Camera.main.transform.forward;
        }
    }
}