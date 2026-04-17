using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

    public void UpdateEnergy(int value)
    {
        energyText.text = "Energía: " + value;
    }

    public void UpdateWood(int value)
    {
        woodText.text = "Madera: " + value;
    }

    public void UpdateStone(int value)
    {
        stoneText.text = "Piedra: " + value;
    }
}