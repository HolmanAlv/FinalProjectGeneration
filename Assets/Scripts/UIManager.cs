using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI NigthsComplete;

    public void UpdateEnergy(int value)
    {
        energyText.text = value.ToString();
    }

    public void UpdateWood(int value)
    {
        woodText.text = value.ToString();
    }

    public void UpdateStone(int value)
    {

        //stoneText.text = "Piedra: " + value;
        stoneText.text = value.ToString();
    }

    public void UpdateNigth(int value)
    {

        //stoneText.text = "Piedra: " + value;
        NigthsComplete.text = "Noches completadas: " + value;
    }
}