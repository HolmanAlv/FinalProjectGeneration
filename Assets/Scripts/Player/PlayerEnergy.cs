using NUnit.Framework;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    public int maxEnergy = 20;
    public int currentEnergy = 0;

    public UIManager uIManager;

    public bool HasEnoughEnergy(int amount)
    {
        return currentEnergy >= amount;
    }

    public void AddEnergy(int amount)
    {
        currentEnergy += amount;

        if(currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }

        uIManager.UpdateEnergy(currentEnergy);
        Debug.Log("Energia actual:" + currentEnergy);
    }

    public bool ConsumeEnergy(int amount)
    {
        if(currentEnergy < amount)
        return false;

        currentEnergy -= amount;
        uIManager.UpdateEnergy(currentEnergy);
        Debug.Log("Energia restante:" + currentEnergy);
        return true;

    }
    
  
}
