using UnityEngine;
using UnityEngine.UI;

public class MainHealthUI : MonoBehaviour
{
    public Image healthFill;

    public void UpdateHealth(float current, float max)
    {
        healthFill.fillAmount = current / max;
    }
}